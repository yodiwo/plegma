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
    public class InboxService : IInbox
    {
        private string ServiceUrl = "http://localhost:8080/rest/inbox";
        private readonly HttpClient client = new HttpClient();
        public InboxService(string url)
        {
            this.ServiceUrl = String.Format("{0}/rest/inbox", url);
        }
        public IEnumerable<DiscoveryResult> All()
        {
            try
            {
                HttpResponseMessage response = client.GetAsync(ServiceUrl).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<DiscoveryResult[]>(jsonString);
            }
            catch (Exception)
            { return null; }
        }
        public bool Approve(string id)
        {
            try
            {
                var content = new StringContent("", Encoding.UTF8, "text/plain");
                //TODO: check version
                HttpResponseMessage response = client.PostAsync(String.Format("{0}/{1}/approve", ServiceUrl, id), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public bool Ignore(string id)
        {
            try
            {
                var content = new StringContent("", Encoding.UTF8, "text/plain");
                //TODO: check version
                HttpResponseMessage response = client.PostAsync(String.Format("{0}/{1}/ignore", ServiceUrl, id), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public bool Remove(string id)
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
    }
}
