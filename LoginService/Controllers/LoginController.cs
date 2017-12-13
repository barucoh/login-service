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
        static Dictionary<string, Token> ActiveLogins = new Dictionary<string, Token>();
        static List<User> Users = new List<User>();
        // GET api/login
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/login/5
        // Returns TTL of User's token
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet]
        [Route("ValidateSession/{tokenId}")]
        public async Task<Boolean> ValidateSession(string tokenId)
        {
            var httpClient = Helpers.CouchDBConnect.GetClient("users");
            var response = await httpClient.GetAsync("users/" + tokenId);
            if (!response.IsSuccessStatusCode)
                return false;
            var token = (Token)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), typeof(Token));
            return token.createdTime.AddSeconds(token._ttl).CompareTo(DateTime.Now) > 0;
        }

        // POST api/login
        [HttpPost]
        public async Task<Token> Post([FromBody]User user)
        {
            var httpClient = Helpers.CouchDBConnect.GetClient("users");
            var response = await httpClient.GetAsync("users/" + user._id);
            if (response.IsSuccessStatusCode)
            {
                User u = (User)JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), typeof(User));
                if(user.password.Equals(u.password))
                {
                    Token token = new Token();
                    token._id = user._id + ":token:" + Guid.NewGuid();
                    token._ttl = 600;
                    token.createdTime = DateTime.Now;

                    HttpContent httpContent = new StringContent(
                        JsonConvert.SerializeObject(token),
                        System.Text.Encoding.UTF8,
                        "application/json"
                        );

                    await httpClient.PostAsync("users", httpContent);

                    return token;
                }
            }
            return null;
        }

        async Task<Boolean> DoesUserExist(User user)
        {
            var httpClient = Helpers.CouchDBConnect.GetClient("users");
            var response = await httpClient.GetAsync("users/" + user._id);
            return response.IsSuccessStatusCode ? false : true;
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<int> CreateUser([FromBody] User user)
        {
            if (await DoesUserExist(user))
                return -1;
            var httpClient = Helpers.CouchDBConnect.GetClient("users");
            string jsonifiedUserObject = JsonConvert.SerializeObject(user);
            HttpContent httpContent = new StringContent(jsonifiedUserObject, System.Text.Encoding.UTF8, "applications/json");
            var response = await httpClient.PostAsync("", httpContent);
            Console.WriteLine(response);

            return 0;
        }

        // PUT api/login/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/login/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
