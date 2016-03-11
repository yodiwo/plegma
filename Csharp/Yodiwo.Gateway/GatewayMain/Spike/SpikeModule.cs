using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.API.Spike;
using Yodiwo.NodeLibrary;
using Yodiwo.Spike;

namespace Yodiwo.Gateway.GatewayMain.Spike
{
    class SpikeModule : Yodiwo.NodeLibrary.INodeModule
    {
        #region variables

        public Yodiwo.NodeLibrary.Node Node { get; set; }

        private readonly DictionaryTS<SpikeNode, GatewayLink> _spikeNodes = new DictionaryTS<SpikeNode, GatewayLink>();

        private readonly DictionaryTS<Thing, TupleS<SpikeNode, uint>> _things = new DictionaryTS<Thing, TupleS<SpikeNode, uint>>();

        private readonly SpikeConfigurator _configurator;

        #endregion

        #region constructors

        public SpikeModule()
        {
            _configurator = SpikeConfigurator.InitConfigurator();
        }
        #endregion

        #region functions

        public void AddSpikeNode(SpikeNode sNode, GatewayLink link)
        {
            //revisit
            _spikeNodes[sNode] = link;
            sNode.SpikeMessageSender = link.SendMessage;
            foreach (var spikeThing in sNode.SpikeThings)
            {
                var thing = SpikeThing.ThingFromSpikeThing(spikeThing, sNode.SpikeNodeName, sNode.SubNodeId);
                _things[thing] = new TupleS<SpikeNode, uint>(sNode, spikeThing.SpikeThingId);
            }
        }

        public int GetSubNodeId(DeviceKey devKey)
        {
            int id;
            if (_configurator.ActiveConfig.AssignedIds.TryGetValue(devKey, out id))
                return id;
            if (_configurator.ActiveConfig.AssignedIds.Count > 0)
                id = _configurator.ActiveConfig.AssignedIds.Values.Max() + 1;
            else
                id = 1;
            _configurator.ActiveConfig.AssignedIds[devKey] = id;
            return id;
        }

        public void AddSampleNode(GatewayLink link)
        {
            var devKey = new DeviceKey
            {
                PlatformId = new PlatformId
                {
                    ComId = 1,
                    FamilyId = 2,
                    McuId = 3,
                },
                UniqueId = 1337,
            };
            var snid = GetSubNodeId(devKey);

            var node = new SpikeNode(SampleSpikeThingDefinitions.SpikeThings, "nxp", snid, _configurator);
            AddSpikeNode(node, link);
        }

        public bool Delete(Thing[] thing)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Thing> EnumerateThings()
        {
            return _things.Keys;
        }

        public IEnumerable<ThingType> EnumerateThingTypes()
        {
            var stypes = new HashSet<string>();
            foreach (var thing in _things.Keys)
            {
                stypes.Add(thing.Type);
            }
            return stypes.Select(t => new ThingType
            {
                Type = t,
                Description = t,
                Searchable = false,
                Model = new ThingModelType[0],
            });
        }

        public IEnumerable<Thing> Scan(ThingKey tkey)
        {
            return new List<Thing>();
        }

        public void OnThingUpdated(Thing thing, Thing oldCopy)
        {
            if (thing.Config == null)
                return;
            var updatedConfs = thing.Config.Where(newconf => oldCopy.Config.Any(oldconf => oldconf.Name == newconf.Name && oldconf.Value != newconf.Value));
            var tk = (ThingKey)thing.ThingKey;
            var spikeNode = GetSpikeNode(tk);
            spikeNode.OnSpikeThingUpdated(tk.SpikeThingId, updatedConfs);
        }

        #region PortEvents
        public void SetThingsState(PortKey pk, string state)
        {
            var spikeNode = GetSpikeNode(pk);
            if (spikeNode == null)
            {
                DebugEx.TraceWarning($"Did not find spikenode associated with portkey {pk}");
                return;
            }
            var thing = _things.Keys.FirstOrDefault(t => t.ThingKey == pk.ThingKey);
            var port = thing?.GetPort(pk);
            var portIdx = thing.Ports.IndexOf(port);
            if (port == null || portIdx < 0)
                return;


            spikeNode.OnChangedState(pk.ThingKey.SpikeThingId, (uint)portIdx, state, port.Type);
        }

        public void OnPortEvent(PortEventMsg msg)
        {
            var nodes = new HashSet<SpikeNode>();
            foreach (var ev in msg.PortEvents)
            {
                var node = GetSpikeNode(((PortKey)ev.PortKey));
                nodes.Add(node);
            }
            foreach (var node in nodes)
            {
                node.SendSpikeMessage();
            }
        }
        #endregion

        #region auxiliary functions

        public SpikeNode GetSpikeNode(ThingKey thingKey)
        {
            return _spikeNodes.Keys.FirstOrDefault(n => n.SubNodeId == thingKey.SubnodeId);
        }

        public SpikeNode GetSpikeNode(PortKey portKey)
        {
            return _spikeNodes.Keys.FirstOrDefault(n => n.SubNodeId == portKey.ThingKey.SubnodeId);
        }


        public Thing GetThing(string thingKey)
        {
            return _things.Keys.FirstOrDefault(t => t.ThingKey == thingKey);
        }
        #endregion

        #endregion

    }
}
