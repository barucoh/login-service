using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using LoginService.Models;

namespace LoginService.Helpers
{
    public class CouchDBConnect
    {
        private static string host = "server1";
        public static HttpClient GetClient(string db)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(host);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        private void Post(User user)
        {

        }
    }
}
