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
    public class LinkService : ILink
    {
        private string ServiceUrl = "http://localhost:8080/rest/links";
        private readonly HttpClient client = new HttpClient();
        public LinkService(string url)
        {
            this.ServiceUrl = String.Format("{0}/rest/links", url);
        }
        public IEnumerable<Link> All()
        {
            try
            {
                HttpResponseMessage response = client.GetAsync(ServiceUrl).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<Link[]>(jsonString);
            }
            catch (Exception)
            { return null; }
        }
        public bool Link(string name, string channel)
        {
            try
            {
                var content = new StringContent("", Encoding.UTF8, "text/plain");
                HttpResponseMessage response = client.PutAsync(String.Format("{0}/{1}/{2}", ServiceUrl, name, channel), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public bool UnLink(string name, string channel)
        {
            try
            {
                HttpResponseMessage response = client.DeleteAsync(String.Format("{0}/{1}/{2}", ServiceUrl, name, channel)).Result;
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
