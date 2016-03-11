using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.Gateway.Openhab.Rest.Model;

namespace Yodiwo.Gateway.Openhab.Rest.Services
{
    public interface IThing
    {
        IEnumerable<Thing> All();
        Thing Get(string id);
        bool Delete(string id);
        bool Add(Thing thing);
        bool Update(Thing thing);
        bool Link(string id, string channel, string item);
        bool UnLink(string id, string channel, string item);
    }
}
