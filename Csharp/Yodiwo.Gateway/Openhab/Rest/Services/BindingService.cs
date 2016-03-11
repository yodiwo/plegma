using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Gateway.Openhab.Rest.Model;

namespace Yodiwo.Gateway.Openhab.Rest.Services
{
    class BindingService : IBinding
    {
        private string ServiceUrl = "http://localhost:8080/rest/bindings";
        private readonly HttpClient client = new HttpClient();
        public BindingService(string url)
        {
            this.ServiceUrl = String.Format("{0}/rest/bindings", url);
        }
        public IEnumerable<Binding> All()
        {
            try
            {
                HttpResponseMessage response = client.GetAsync(ServiceUrl).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<Binding[]>(jsonString);
            }
            catch (Exception)
            { return null; }
        }
    }
}
