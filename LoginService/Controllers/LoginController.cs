using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginService.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace LoginService.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        // GET api/login/validatesession/<user>:token:<guid>
        // Returns TTL of User's token
        [HttpGet]
        [Route("ValidateSession/{tokenId}")]
        public async Task<Boolean> ValidateSession(string tokenId)
        {
            var httpClient = Helpers.CouchDBConnect.GetClient("users");
            var response = await httpClient.GetAsync("users/" + tokenId);
            if (!response.IsSuccessStatusCode)
                return false;
            var token = (Token)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), typeof(Token));
            return token.CreatedTime.AddSeconds(token.TTL).CompareTo(DateTime.Now) > 0;
        }

        // POST api/login
        // User logging in and new token is created
        [HttpPost]
        public async Task<User> Post([FromBody]User user)
        {
            var httpClient = Helpers.CouchDBConnect.GetClient("users");
            User u = await DoesUserExist(user._id);
            if (u != null)
            {
                if (user.password.Equals(u.password))
                {
                    u.token = new Token();
                    u.token.ID = u._id + ":token:" + Guid.NewGuid();
                    u.token.TTL = 600;
                    u.token.CreatedTime = DateTime.Now;

                    HttpContent httpContent = new StringContent(
                        JsonConvert.SerializeObject(u),
                        System.Text.Encoding.UTF8,
                        "application/json"
                        );

                    var response = await httpClient.PutAsync("users/" + u._id, httpContent);

                    return u;
                }
            }
            return null;
        }

        // POST api/login/createuser
        // Creating a new user
        [HttpPost]
        [Route("CreateUser")]
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
                user.token.CreatedTime = System.DateTime.Now;
                string jsonifiedUserObject = JsonConvert.SerializeObject(user);
                HttpContent httpContent = new StringContent(
                    jsonifiedUserObject,
                    System.Text.Encoding.UTF8,
                    "application/json"
                    );
                var response = await httpClient.PutAsync("users/" + id, httpContent);
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
