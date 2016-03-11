using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Helpers;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;

namespace Yodiwo.Gateway.GatewayMain.Nest
{
    public class NestBindingModule : NancyModule
    {
        public NestBindingModule()
        {
#if false
            Get["/nest"] = parameters =>
                {
                    return "OK";
                };
            Get["/nest/api/oauth"] = parameters =>
            {
                var code = (string)this.Request.Query["code"];
                var accessToken = GetAccessToken(code, NestModule.clientId, NestModule.clientSecret);
                if (NestModule.OnAccessTokenRxCb != null)
                    NestModule.OnAccessTokenRxCb(accessToken);
                return "OK";
            };

        }

        public void NestLogin(string clientId)
        {
            var authorizationUrl = string.Format("https://home.nest.com/login/oauth2?client_id={0}&state={1}",
               clientId, "yodiwo-auth");

            using (var process = Process.Start(authorizationUrl))
            {
                Console.WriteLine("Please Authenticate the yodiwo App");
            }
        }

        private string GetAccessToken(string clientid, string clientsecret, string authorizationCode)
        {
            var url = string.Format("https://api.home.nest.com/oauth2/access_token?code={0}&client_id={1}&client_secret={2}&grant_type=authorization_code",
                authorizationCode, clientid, clientsecret);

            using (var httpClient = new HttpClient())
            {
                using (var response = httpClient.PostAsync(url, content: null).Result)
                {
                    var accessToken = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);

                    return (accessToken as dynamic).access_token;
                }
            }
        }
#endif

        }
    }
}
