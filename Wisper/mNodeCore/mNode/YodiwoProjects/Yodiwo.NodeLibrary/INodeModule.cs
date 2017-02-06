using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.NodeLibrary
{
    public delegate void OnNodeModuleData1Delegate(PortKey pk, string state);
    public delegate void OnNodeModuleData2Delegate(IEnumerable<TupleS<PortKey, string>> states);
    public delegate void OnNodeModuleData3Delegate(string pk, string state);
    public delegate void OnNodeModuleData4Delegate(IEnumerable<TupleS<string, string>> states);

    public delegate void OnNodeModuleThingAddDelegate(INodeModule Module, IEnumerable<Thing> things);
    public delegate void OnNodeModuleThingDeleteDelegate(INodeModule Module, IEnumerable<ThingKey> things);
    public delegate void OnNodeModuleThingUpdateDelegate(INodeModule Module, IEnumerable<Thing> things, bool sendToCloud = true);

    public interface INodeModule
    {
        string ModuleID { get; set; }
        string Name { get; set; }

        OnNodeModuleData1Delegate OnNodeModuleData1 { get; set; }
        OnNodeModuleData2Delegate OnNodeModuleData2 { get; set; }
        OnNodeModuleData3Delegate OnNodeModuleData3 { get; set; }
        OnNodeModuleData4Delegate OnNodeModuleData4 { get; set; }

        OnNodeModuleThingAddDelegate OnNodeModuleThingAdd { get; set; }
        OnNodeModuleThingDeleteDelegate OnNodeModuleThingDelete { get; set; }
        OnNodeModuleThingUpdateDelegate OnNodeModuleThingUpdate { get; set; }

        NodeKey NodeKey { get; set; }

        IEnumerable<ThingType> EnumerateThingTypes();

        void SetThingsState(PortKey pk, string state, bool isEvent);
        void SetThingsState(ThingKey thingKey, List<TupleS<PortKey, string>> states, bool isEvent);

        IEnumerable<Thing> Scan(ThingKey tkey);

        bool Delete(ThingKey[] thing);
        void OnUpdateThing(Thing thing, Thing oldCopy);

        void OnPortEvent(PortEventMsg msg);

        void OnTransportConnected(string msg);
        void OnTransportDisconnected(string msg);
        void OnLinkActivated();

        void OnThingActivated(ThingKey thingKey);
        void OnThingDeactivated(ThingKey thingKey);
        void OnPortActivated(PortKey portKey);
        void OnPortDeactivated(PortKey portKey);

        void Purge();
    }
}
