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
    class ThingTypeService : IThingType
    {
        private string ServiceUrl = "http://localhost:8080/rest/thing-types";
        private readonly HttpClient client = new HttpClient();
        public ThingTypeService(string url)
        {
            this.ServiceUrl = String.Format("{0}/rest/thing-types", url);
        }
        public IEnumerable<ThingType> All()
        {
            try
            {
                HttpResponseMessage response = client.GetAsync(ServiceUrl).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<ThingType[]>(jsonString);
            }
            catch (Exception)
            { return null; }
        }
        public ThingType Get(string id)
        {
            try
            {
                HttpResponseMessage response = client.GetAsync(String.Format("{0}/{1}", ServiceUrl, id)).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<ThingType>(jsonString);
            }
            catch (Exception)
            { return null; }
        }
    }
}
