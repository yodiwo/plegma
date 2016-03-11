using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.Openhab.Rest.Services
{
    public class DiscoveryService : IDiscovery
    {
        private string ServiceUrl = "http://localhost:8080/rest/discovery";
        private readonly HttpClient client = new HttpClient();
        public DiscoveryService(string url)
        {
            this.ServiceUrl = String.Format("{0}/rest/discovery", url);
        }
        public IEnumerable<string> All()
        {
            try
            {
                HttpResponseMessage response = client.GetAsync(ServiceUrl).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<string[]>(jsonString);
            }
            catch (Exception)
            { return null; }
        }
        public bool Scan(string id)
        {
            try
            {
                var content = new StringContent(id, Encoding.UTF8, "text/plain");
                //TODO:version check
                HttpResponseMessage response = client.PostAsync(String.Format("{0}/bindings/{1}/scan", ServiceUrl, id), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
    }
}
