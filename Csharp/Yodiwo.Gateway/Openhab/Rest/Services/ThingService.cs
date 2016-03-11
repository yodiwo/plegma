using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Gateway.Openhab.Rest.Model;
using Newtonsoft.Json;

namespace Yodiwo.Gateway.Openhab.Rest.Services
{
    public class ThingService : IThing
    {
        private string ServiceUrl = "http://localhost:8080/rest/things";
        private readonly HttpClient client = new HttpClient();
        public ThingService(string url)
        {
            this.ServiceUrl = String.Format("{0}/rest/things", url);
        }
        public IEnumerable<Thing> All()
        {
            try
            {
                HttpResponseMessage response = client.GetAsync(ServiceUrl).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<Thing[]>(jsonString);
            }
            catch (Exception)
            { return null; }
        }
        public Thing Get(string id)
        {
            try
            {
                HttpResponseMessage response = client.GetAsync(String.Format("{0}/{1}", ServiceUrl, id)).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<Thing>(jsonString);
            }
            catch (Exception)
            { return null; }
        }
        public bool Delete(string id)
        {
            try
            {
                HttpResponseMessage response = client.DeleteAsync(String.Format("{0}/{1}", ServiceUrl, id)).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public bool Add(Thing thing)
        {
            try
            {
                var jsonString = thing.ToJSON(HtmlEncode: false);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(String.Format("{0}", ServiceUrl), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }

        }
        public bool Update(Thing thing)
        {
            try
            {
                var jsonString = thing.ToJSON(HtmlEncode: false);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsync(String.Format("{0}/{1}", ServiceUrl, thing.UID), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public bool Link(string id, string channel, string item)
        {
            try
            {
                var content = new StringContent(item, Encoding.UTF8, "text/plain");
                HttpResponseMessage response = client.PostAsync(String.Format("{0}/{1}/channels/{2}/link", ServiceUrl, id, channel), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public bool UnLink(string id, string channel, string item)
        {
            try
            {
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Delete, String.Format("{0}/{1}/channels/{2}/link", ServiceUrl, id, channel));
                req.Content = new StringContent(item, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.SendAsync(req, HttpCompletionOption.ResponseContentRead).Result;
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
