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
        private static string host = "https://ba12dc11-6e21-4276-8e2b-6dc0da1d450f-bluemix.cloudant.com";
        public static HttpClient GetClient(string db)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(host);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "YmExMmRjMTEtNmUyMS00Mjc2LThlMmItNmRjMGRhMWQ0NTBmLWJsdWVtaXg6MDRhYTQwNGFjNzdkNDg3YjY5MmQ4YWM0ZWM3YzgwMDZmNmI2YjA1YzkzYTllMzRmYjY0NDM1NjA4NGIzZjUyOQ==");
            return httpClient;
        }

        private void Post(User user)
        {

        }
    }
}
