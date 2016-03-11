using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Gateway.Openhab.Rest.Model;
using Newtonsoft.Json;

namespace Yodiwo.Gateway.Openhab.Rest.Services
{
    public class ItemService : IItem
    {

        private string ServiceUrl = "http://localhost:8080/rest/items";
        private readonly HttpClient client = new HttpClient();
        public ItemService(string url)
        {
            this.ServiceUrl = String.Format("{0}/rest/items", url);
        }

        public IEnumerable<Item> All()
        {
            try
            {
                HttpResponseMessage response = client.GetAsync(ServiceUrl).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<Item[]>(jsonString);
            }
            catch (Exception)
            { return null; }
        }
        public Item Get(string name)
        {
            try
            {
                HttpResponseMessage response = client.GetAsync(String.Format("{0}/{1}", ServiceUrl, name)).Result;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<Item>(jsonString);
            }
            catch (Exception)
            { return null; }
        }
        public bool Create(string name, string type)
        {
            try
            {
                var content = new StringContent(type, Encoding.UTF8, "text/plain");
                HttpResponseMessage response = client.PutAsync(String.Format("{0}/{1}", ServiceUrl, name), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }

        }
        public bool Delete(string name)
        {
            try
            {
                HttpResponseMessage response = client.DeleteAsync(String.Format("{0}/{1}", ServiceUrl, name)).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        /*
        public bool SetState(string name, string state)
        {
            try
            {
                var content = new StringContent(state, Encoding.UTF8, "text/plain");
                HttpResponseMessage response = client.PutAsync(String.Format("{0}/{1}/state", ServiceUrl, name), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
         */
        public string GetState(string name)
        {
            try
            {
                HttpResponseMessage response = client.GetAsync(String.Format("{0}/{1}/state", ServiceUrl, name)).Result;
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    return null;
                var jsonString = response.Content.ReadAsStringAsync().Result;
                return jsonString;
            }
            catch (Exception)
            { return null; }
        }
        public bool SendCommand(string name, string command)
        {
            try
            {
                var content = new StringContent(command, Encoding.UTF8, "text/plain");
                HttpResponseMessage response = client.PostAsync(String.Format("{0}/{1}", ServiceUrl, name), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public bool CreateMember(string name, string member)
        {
            try
            {
                var content = new StringContent("", Encoding.UTF8, "text/plain");
                HttpResponseMessage response = client.PutAsync(String.Format("{0}/{1}/members/{2}", ServiceUrl, name, member), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public bool DeleteMember(string name, string member)
        {
            try
            {
                var content = new StringContent("", Encoding.UTF8, "text/plain");
                HttpResponseMessage response = client.DeleteAsync(String.Format("{0}/{1}/members/{2}", ServiceUrl, name, member)).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public bool CreateTag(string name, string tag)
        {
            try
            {
                var content = new StringContent("", Encoding.UTF8, "text/plain");
                HttpResponseMessage response = client.PutAsync(String.Format("{0}/{1}/tags/{2}", ServiceUrl, name, tag), content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }
        public bool DeleteTag(string name, string tag)
        {
            try
            {
                var content = new StringContent("", Encoding.UTF8, "text/plain");
                HttpResponseMessage response = client.DeleteAsync(String.Format("{0}/{1}/tags/{2}", ServiceUrl, name, tag)).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            { return false; }
        }

        //private string Serialize(Item item)
        //{
        //}
    }
}
