using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.Gateway.Openhab.Rest.Model;
using Yodiwo.Gateway.Openhab.Rest.Services;
using Yodiwo.NodeLibrary;
using StateDescription = Yodiwo.API.Plegma.StateDescription;
using Thing = Yodiwo.API.Plegma.Thing;
using ThingType = Yodiwo.API.Plegma.ThingType;

namespace Yodiwo.Gateway.GatewayMain
{
    public class OpenHubModule : INodeModule
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        HABService openhab;
        //------------------------------------------------------------------------------------------------------------------------
        object locker = new object();
        //------------------------------------------------------------------------------------------------------------------------
        public Yodiwo.NodeLibrary.Node Node { get; set; }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public OpenHubModule(string OpenhabBaseUrl)
        {
            openhab = new HABService(OpenhabBaseUrl);
            OpenHubHost.OnOpenHubRxCb += OnOpenHubBusEventRxcb;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        #region GetThings
        public IEnumerable<Thing> EnumerateThings()
        {
            var list = new List<Yodiwo.API.Plegma.Thing>();

            var data = openhab.Things.All();
            var items = openhab.Items.All();

            if (data != null && items != null)
            {
                list.AddRange(data.Select((t) => t.ToApiThing()));

                foreach (var item in items)
                {
                    bool addit = true;
                    if (item.type == "GroupItem")
                    {
                        continue;
                    }
                    foreach (var thing in data)
                    {
                        if (thing.channels.Count != 0 && thing.channels[0].linkedItems.Count != 0)
                        {
                            foreach (var ch in thing.channels)
                            {
                                if (item.name == ch.linkedItems[0])
                                {
                                    addit = false;
                                }
                            }
                        }
                    }

                    if (addit == true)
                    {
                        DebugEx.TraceLog("item " + item.name);
                        var nthing = new Openhab.Rest.Model.Thing();
                        nthing.UID = item.name;
                        nthing.item = new GroupItem();
                        nthing.item.type = "GroupItem";
                        nthing.item.name = item.name;
                        nthing.item.label = item.name;
                        nthing.item.state = item.state;
                        nthing.item.members.Add(item);
                        nthing.channels = new List<Channel>();

                        Channel ch = new Channel();
                        ch.id = item.name;
                        ch.linkedItems.Add(item.link);
                        ch.itemType = item.type;
                        nthing.channels.Add(ch);
                        nthing.item.members.Add(item);

                        list.Add(nthing.ToApiThing());
                        var d = data;
                    }
                }
            }
            return list;
        }
        #endregion
        //------------------------------------------------------------------------------------------------------------------------
        #region GetThingTypes
        public IEnumerable<ThingType> EnumerateThingTypes()
        {
            var bindings = openhab.Bindings.All();
            var discovery = openhab.Discovery.All();

            var thingTypes = new List<Yodiwo.API.Plegma.ThingType>();

            if (bindings != null)
            {
                foreach (var binding in bindings)
                {
                    var Models = new List<ThingModelType>();

                    var type = new Yodiwo.API.Plegma.ThingType
                    {
                        Type = binding.id,
                        Searchable = discovery.Contains(binding.id) ? true : false,
                        Description = binding.description,

                    };

                    foreach (var t in binding.thingTypes)
                    {
                        var model = new ThingModelType
                        {
                            Id = t.UID,
                            Name = t.label,
                            Description = t.description,
                        };

                        model.Config = t.configParameters.Select(c => new ConfigDescription
                        {
                            DefaultValue = c.defaultValue,
                            Description = c.description,
                            Label = c.label,
                            Name = c.name,
                            Required = c.required,
                            Type = c.type.ToString(),
                            Minimum = c.minimum,
                            Maximum = c.maximum,
                            Stepsize = c.stepsize,
                            ReadOnly = c.readOnly,
                        }).ToArray();

                        model.Port = t.channels.Select(p => new PortDescription
                        {
                            Description = p.description,
                            Id = p.id,
                            Label = p.label,
                            Category = p.category,
                            State = new Yodiwo.API.Plegma.StateDescription
                            {
                                Minimum = p.stateDescription.minimum,
                                Maximum = p.stateDescription.maximum,
                                Step = p.stateDescription.step,
                                Pattern = p.stateDescription.pattern,
                                ReadOnly = p.stateDescription.readOnly,
                            },
                        }).ToArray();

                        Models.Add(model);
                    }

                    type.Model = Models.ToArray();
                    thingTypes.Add(type);
                }
            }
            return thingTypes;
        }
        #endregion
        //------------------------------------------------------------------------------------------------------------------------
        #region PortEvents
        public void SetThingsState(PortKey portKey, string state)
        {
            var id = "";
            //ThingUID == PortUID means that this is a 1.7
            //openhab item manually converted to thing
            if (portKey.ThingKey.ThingUID == portKey.PortUID)
            {
                id = portKey.PortUID;
                var data = openhab.Items.Get(id);
                if (data != null)
                {
                    switch (data.type)
                    {
                        case "SwitchItem":
                            if (state.Equals("0"))
                            {
                                state = "OFF";
                            }
                            else
                            {
                                state = "ON";
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                id = (portKey.ThingKey.ThingUID + ":" + portKey.PortUID).Replace(':', '_');
            }
            var ret = openhab.Items.SendCommand(id, state.ToUpper());
            DebugEx.TraceLog("GW Item  " + id + " - cmd " + state.ToUpper() + " ret=" + ret);
        }


        public void OnPortEvent(PortEventMsg msg)
        {
        }
        #endregion
        //------------------------------------------------------------------------------------------------------------------------
        #region OnBusEvent
        public void OnOpenHubBusEventRxcb(string openhubthing, string channelid, string state)
        {
            var tk = new ThingKey(GatewayStatics.ActiveCfg.NodeKey, openhubthing);
            var pk = new PortKey(tk, channelid);
            Node?.SetState(pk, state);
        }
        #endregion
        //------------------------------------------------------------------------------------------------------------------------
        #region Scan
        public IEnumerable<Yodiwo.API.Plegma.Thing> Scan(ThingKey key)
        {
            List<Yodiwo.API.Plegma.Thing> GotThings = null;

            var ret = openhab.Discovery.Scan(key.ThingUID);

            var found = new List<string>();
            bool bridge = false;
            if (ret)
            {
                var inbox = openhab.Inbox.All();
                foreach (var discoveryResult in inbox)
                {
                    ret = openhab.Inbox.Approve(discoveryResult.thingUID);
                    if (ret)
                    {
                        if (discoveryResult.thingUID.Split(':')[1].Equals("bridge"))
                        {
                            bridge = true;
                        }
                        else
                        {
                            found.Add(discoveryResult.thingUID);
                        }

                    }
                }
                //NOTE: In case of bridges attached devices added to inbox after bridge abroval 
                //
                //System.Threading.Task.Delay(5000).Wait();
                if (bridge)
                {
                    ret = openhab.Discovery.Scan(key.ThingUID);
                    inbox = openhab.Inbox.All();
                    bridge = false;
                    foreach (var discoveryResult in inbox)
                    {
                        ret = openhab.Inbox.Approve(discoveryResult.thingUID);
                        if (ret)
                        {
                            if (discoveryResult.thingUID.Split(':')[1].Equals("bridge"))
                            {
                                bridge = true;
                            }
                            else
                            {
                                found.Add(discoveryResult.thingUID);
                            }

                        }
                        if (ret) found.Add(discoveryResult.thingUID);
                    }
                }


                var things = openhab.Things.All();
                if (things != null)
                {
                    if (found.Any())
                    {
                        GotThings = things.Where(x => found.Contains(x.UID)).Select((t) => t.ToApiThing()).ToList();
                    }
                }
            }
            return GotThings;
        }
        #endregion
        //------------------------------------------------------------------------------------------------------------------------
        #region Update
        private bool UpdateThings(Thing[] incomingThings)
        {
            bool created = false;

            foreach (var inThing in incomingThings)
            {
                ThingKey tkey = inThing.ThingKey;
                var ohThing = openhab.Things.Get(tkey.ThingUID);
                if (ohThing == null)
                {
                    ohThing = new Yodiwo.Gateway.Openhab.Rest.Model.Thing();
                    ohThing.UID = tkey.ThingUID;
                    created = true;
                }

                ohThing.item.label = inThing.Name;

                if (ohThing.configuration.Any())
                    ohThing.configuration.Clear();

                foreach (var cfg in inThing.Config)
                {
                    ohThing.configuration.Add(cfg.Name, cfg.Value);
                }

                var ret = (created) ?
                              openhab.Setup.AddThing(ohThing) :
                              openhab.Setup.UpdateThing(ohThing);
                if (!ret)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
        //------------------------------------------------------------------------------------------------------------------------
        #region ThingsDelete
        public bool Delete(Thing[] incomingThings)
        {
            foreach (var inThing in incomingThings)
            {
                ThingKey tkey = inThing.ThingKey;
                var ret = openhab.Setup.DeleteThing(tkey.ThingUID);
                if (!ret)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}

