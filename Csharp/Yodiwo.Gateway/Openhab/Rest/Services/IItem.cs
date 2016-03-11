using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Gateway.Openhab.Rest.Model;

namespace Yodiwo.Gateway.Openhab.Rest.Services
{
    public interface IItem
    {
        IEnumerable<Item> All();
        Item Get(string name);
        bool Create(string name, string type);
        bool Delete(string name);
        //bool SetState(string name, string state);
        string GetState(string name);
        bool SendCommand(string name, string command);
        bool CreateMember(string name, string member);
        bool DeleteMember(string name, string member);
        bool CreateTag(string name, string tag);
        bool DeleteTag(string name, string tag);
    }
}
