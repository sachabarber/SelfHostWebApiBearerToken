using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;


namespace Davide.Trotta.WebApi.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = WebApp.Start<Startup>(url: "http://localhost:9000/");
            Console.WriteLine("Web API listening at http://localhost:9000/");



            using (var client = new HttpClient())
            {

                //Will call OAuthAuthorizationServerProvider.ValidateClientAuthentication
                var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes("rajeev:secretKey"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);  
  

                var form = new Dictionary<string, string>  
               {  
                   {"grant_type", "password"},  
                   {"username", "rranjan"},  
                   {"password", "password@123"},  
               };

                var tokenResponse = client.PostAsync("http://localhost:9000/token", new FormUrlEncodedContent(form)).Result;


                var token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;


                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

                var result = client.GetAsync("http://localhost:9000/api/Details").Result;
                string resultContent = result.Content.ReadAsStringAsync().Result;



                Console.WriteLine("");
                Console.ReadLine();
            }

        }


        public class Token
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
        }  
    }
}
