using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Gateway.Openhab.Rest.Model;

namespace Yodiwo.Gateway.Openhab.Rest.Services
{
    public interface ISetup
    {
        IEnumerable<Thing> AllThing();
        bool DeleteThing(string id);
        bool AddThing(Thing thing);
        bool UpdateThing(Thing thing);
        bool EnableChannel(string id);
        bool DisableChannel(string id);
        bool SetLabel(string id, string label);
        IEnumerable<GroupItem> AllGroup();
        bool AddGroup(Item item);
        bool DeleteGroup(string id);
    }
}
