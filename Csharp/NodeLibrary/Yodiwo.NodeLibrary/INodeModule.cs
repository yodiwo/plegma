using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.Node.Module
{
    public delegate void OnDataRxEventHandler(object sender, DataRxEvent data);

    public interface INodeModule
    {
        IEnumerable<Thing> EnumerateThings();
        IEnumerable<ThingType> EnumerateThingTypes();
        void SetThingsState(PortKey pk, string state);
        event OnDataRxEventHandler OnDataRx;
        IEnumerable<Thing> Scan(ThingKey tkey);
        bool Delete(Thing[] thing);
    }

    public class DataRxEvent
    {
        public INodeModule module;
        public PortKey pk;
        public string state;
    }
}
