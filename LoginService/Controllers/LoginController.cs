using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginService.Models;
using Newtonsoft.Json;
using System.Net.Http;
using StackExchange.Redis;
using LoginService.Helpers;

namespace LoginService.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        IDatabase cachingDB;
        public LoginController(IRedisConnectionFactory caching)
        {
            cachingDB = caching.Connection().GetDatabase();
        }

        // GET api/login/validatesession/<user>:token:<guid>
        // Returns TTL of User's token
        [HttpGet]
        [Route("ValidateSession/{userID}")]
        public Boolean ValidateSession(string userID)
        {
            Token token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(cachingDB.StringGet(userID.ToString()));
            if (token == null)
                return false;
            return token.CreatedTime.AddSeconds(token.TTL).CompareTo(DateTime.Now) > 0;
        }

        // POST api/login
        // User logging in and new token is created
        [HttpPost]
        public async Task<System.Net.HttpStatusCode> LoginUser([FromBody]User user)
        {
            var httpClient = Helpers.CouchDBConnect.GetClient("users");
            User u = await DoesUserExist(user._id);
            if (u != null)
            {
                if (user.password.Equals(u.password))
                {
                    Token token = new Token();
                    token.ID = u._id + ":token:" + Guid.NewGuid();
                    token.TTL = 600;
                    token.CreatedTime = DateTime.Now;

                    this.cachingDB.StringSet(u._id.ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(token));

                    return System.Net.HttpStatusCode.Created;
                }
            }
            return System.Net.HttpStatusCode.NotFound;
        }

        // POST api/login/createuser
        // Creating a new user
        [HttpPost]
        [Route("createuser")]
        public async Task<int> CreateUser([FromBody] User user)
        {
            User u = await DoesUserExist(user._id);
            if (u != null)
                return -1;
            var httpClient = Helpers.CouchDBConnect.GetClient("users");
            string jsonifiedUserObject = JsonConvert.SerializeObject(
                new
                {
                    _id = user._id,
                    password = user.password
                });
            HttpContent httpContent = new StringContent(
                jsonifiedUserObject,
                System.Text.Encoding.UTF8,
                "application/json"
                );
            var response = await httpClient.PostAsync("users", httpContent);
            Console.WriteLine(response);
            return 0;
        }

        // PUT api/login/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/login/5
        // Deleting user
        [HttpDelete("{id}")]
        public async void Delete(string id)
        {
            var httpClient = Helpers.CouchDBConnect.GetClient("users");
            User u = await DoesUserExist(id);
            if (u != null)
                await httpClient.DeleteAsync("users/" + u._id + "?rev=" + u._rev);
        }

        // PUT api/login/updatesession/<user id>
        // Renewing user's token
        [HttpPut("{id}")]
        [Route("UpdateSession")]
        public async void UpdateSession(string id)
        {
            var httpClient = Helpers.CouchDBConnect.GetClient("users");
            User user = await DoesUserExist(id);
            if (user != null)
            {
                Token token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(cachingDB.StringGet(user._id.ToString()));
                token.CreatedTime = System.DateTime.Now;
                this.cachingDB.StringSet(user._id.ToString(), Newtonsoft.Json.JsonConvert.SerializeObject(token));
            }
        }

        // Validating user's existence in the database
        async Task<User> DoesUserExist(string id)
        {
            var httpClient = Helpers.CouchDBConnect.GetClient("users");
            var response = await httpClient.GetAsync("users/" + id);
            return response.IsSuccessStatusCode ? (User)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), typeof(User)) : null;
        }
    }
}
