using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.NodeLibrary
{
    public interface INodeModule
    {
        Node Node { get; set; }

        IEnumerable<Thing> EnumerateThings();

        IEnumerable<ThingType> EnumerateThingTypes();

        void SetThingsState(PortKey pk, string state);

        IEnumerable<Thing> Scan(ThingKey tkey);

        bool Delete(Thing[] thing);

        void OnPortEvent(PortEventMsg msg);
    }
}
