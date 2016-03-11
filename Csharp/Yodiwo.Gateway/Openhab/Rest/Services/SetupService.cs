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
    public class SetupService : ISetup
    {
        private string ServiceUrl = "http://localhost:8080/rest/setup";
        private readonly HttpClient client = new HttpClient();
        public SetupService(string url)
        {
            this.ServiceUrl = String.Format("{0}/rest/setup", url);
        }
        public IEnumerable<Thing> AllThing()
        {
            try
            {
                HttpResponseMessage response = client.GetAsync(String.Format("{0}/things", ServiceUrl)).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<Thing[]>(jsonString);
            }
            catch (Exception)
            { return null; }
        }
        public bool DeleteThing(string id)
        {
            try
            {
                HttpResponseMessage response = client.DeleteAsync(String.Format("{0}/things/{1}", ServiceUrl, id)).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public bool AddThing(Thing thing)
        {
            try
            {
                var jsonString = thing.ToJSON(HtmlEncode: false);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(String.Format("{0}/things", ServiceUrl), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; };
        }
        public bool UpdateThing(Thing thing)
        {
            try
            {
                var jsonString = thing.ToJSON(HtmlEncode: false);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsync(String.Format("{0}/things", ServiceUrl), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public bool EnableChannel(string id)
        {
            try
            {
                var content = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsync(String.Format("{0}/things/channels/{1}", ServiceUrl, id), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public bool DisableChannel(string id)
        {
            try
            {
                HttpResponseMessage response = client.DeleteAsync(String.Format("{0}/things/channels/{1}", ServiceUrl, id)).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public bool SetLabel(string id, string label)
        {
            try
            {
                var content = new StringContent(label, Encoding.UTF8, "text/plain");
                HttpResponseMessage response = client.PutAsync(String.Format("{0}/labels/{1}", ServiceUrl, id), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public IEnumerable<GroupItem> AllGroup()
        {
            try
            {
                HttpResponseMessage response = client.GetAsync(String.Format("{0}/groups", ServiceUrl)).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<GroupItem[]>(jsonString);
            }
            catch (Exception)
            { return null; }
        }
        public bool AddGroup(Item item)
        {
            try
            {
                var jsonString = item.ToJSON(HtmlEncode: false);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(String.Format("{0}/groups", ServiceUrl), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; };
        }
        public bool DeleteGroup(string id)
        {
            try
            {
                HttpResponseMessage response = client.DeleteAsync(String.Format("{0}/groups/{1}", ServiceUrl, id)).Result;
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
