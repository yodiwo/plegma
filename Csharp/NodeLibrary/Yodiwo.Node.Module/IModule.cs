using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.Node.Module
{
    public delegate void OnModuleBusEventHandler(object sender, BusEvent data);
    public interface IModule
    {
        List<Thing> EnumerateThings();
        List<ThingType> EnumerateThingTypes();
        void SetThingsState(PortKey pk, string state);
        event OnModuleBusEventHandler OnModuleBusDataRx;
        List<Thing> Scan(ThingKey tkey);
        bool Delete(Thing[] thing);
    }

    public class BusEvent
    {
        public IModule module;
        public PortKey pk;
        public string state;
    }
}
