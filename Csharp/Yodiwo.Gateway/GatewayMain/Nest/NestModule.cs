using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseSharp.Portable;
using Yodiwo;
using System.Net.Http;
using Yodiwo.API.Plegma;
using Yodiwo.NodeLibrary;

namespace Yodiwo.Gateway.GatewayMain.Nest
{
    public class NestModule : INodeModule
    {
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnAccessTokenRx(string accesstoken);
        public static OnAccessTokenRx OnAccessTokenRxCb = null;
        public static string clientId;
        public static string clientSecret;
        public FirebaseApp firebaseClient;
        public static string accessToken = "c.cwMGb6WJVX9coH38BwU7HZwuiaWtMeRBVt35R3C71yWyX4ClENw3p11NAnBCvbqFzqVv438VeYgZCwH6JqhOTHk53N2ZOi0NMa6uM1yqt4D7ABRragmLQEpuWYcJLNmVlDjjfTeR4IWvOYGC";
        //------------------------------------------------------------------------------------------------------------------------
        DictionaryTS<string, Thing> YodiwoNestThings = new DictionaryTS<string, Thing>();
        public static DictionaryTS<string, NestDescriptor> nestdescriptors = new DictionaryTS<string, NestDescriptor>();
        //------------------------------------------------------------------------------------------------------------------------
        public Yodiwo.NodeLibrary.Node Node { get; set; }
        //------------------------------------------------------------------------------------------------------------------------
        static Dictionary<string, Func<IDataSnapshot, List<Thing>>> Creators = new Dictionary<string, Func<IDataSnapshot, List<Thing>>>()
        {
            {"cameras",NestCamera.Convert2YodiwoThing },
            {"thermostats",NestThermostat.Convert2YodiwoThing },
            {"smoke_co_alarms",NestSmokeCoAlarm.Convert2YodiwoThing }
        };
        //------------------------------------------------------------------------------------------------------------------------


        //------------------------------------------------------------------------------------------------------------------------
        public void Init()
        {
            firebaseClient = new FirebaseApp(new Uri("https://developer-api.nest.com/devices/"), accessToken);
            firebaseClient.Child("/devices").On("child_added", (snap, previous_child, context) => Added(snap, context));
            firebaseClient.Child("/devices").On("child_changed", (snap, previous_child, context) => Changed(snap, context));
            firebaseClient.Child("/devices").On("child_removed", (snap, previous_child, context) => Removed(snap, context));
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void Added(IDataSnapshot snap, object context)
        {
            var path = snap.ExportVal();
            var val = snap.Value();
            var things = Creators[snap.Key](snap);
            Add2ThingsDictionary(things);
            things.ForEach(th => Node.AddThing(th));
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void Changed(IDataSnapshot snap, object context)
        {
            var path = snap.ExportVal();
            Console.WriteLine(snap.Key);
            var val = snap.Value();
            Thing thing = null;
            List<TupleS<Port, string>> states = new List<TupleS<Port, string>>();

            var cnt = snap.Children.Count();
            var storedcnt = nestdescriptors.Where(i => i.Value.type.ToString().ToLowerInvariant() == snap.Key).Count();
            //add new things
            if (cnt > nestdescriptors.Where(i => i.Value.type.ToString().ToLowerInvariant() == snap.Key).Count())
            {
                var things = Creators[snap.Key](snap);
                Add2ThingsDictionary(things);
                things.ForEach(th => Node.AddThing(th));
            }
            //remove things
            else if (cnt < nestdescriptors.Where(i => i.Value.type.ToString().ToLowerInvariant() == snap.Key).Count())
            {
                List<string> thingscurrent = new List<string>();
                snap.Children.ForEach(dev => thingscurrent.Add(dev.Key));
                var alldevs = nestdescriptors.Where(i => i.Value.type.ToString().ToLowerInvariant() == snap.Key);
                var alldevkeys = (from kvp in alldevs select kvp.Key.Replace('_', PlegmaAPI.KeySeparator)).ToList();
                var toberemoved = alldevkeys.Except(thingscurrent).ToList();
                foreach (var thing2remove in toberemoved)
                {
                    thing2remove.Replace(PlegmaAPI.KeySeparator, '_');
                    if (YodiwoNestThings.ContainsKey(thing2remove))
                    {
                        Node.RemoveThing(YodiwoNestThings[thing2remove].ThingKey);
                        YodiwoNestThings.Remove(thing2remove);
                    }
                    if (nestdescriptors.ContainsKey(thing2remove))
                    {
                        nestdescriptors.Remove(thing2remove);
                    }
                }
            }
            //set thing's state
            foreach (var device in snap.Children)
            {
                if (YodiwoNestThings.TryGetValue(device.Key, out thing))
                {
                    foreach (var snapchild in device.Children)
                    {
                        var port = thing.Ports.FirstOrDefault(p => ((PortKey)p.PortKey).PortUID == snapchild.Key);
                        if (port != null)
                        {
                            var tuple = new TupleS<Port, string>(port, snapchild.Value());
                            states.Add(tuple);
                        }
                    }
                    Node.SetState(states);
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void Removed(IDataSnapshot snap, object context)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void Add2ThingsDictionary(List<Thing> things)
        {
            foreach (var th in things)
            {
                //var devid = ((ThingKey)th.ThingKey).ThingUID;
                var devid = th.ThingKey.RightOf("$NodeKey$" + PlegmaAPI.KeySeparator);
                if (!YodiwoNestThings.ContainsKey(devid))
                    YodiwoNestThings.Add(devid, th);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public IEnumerable<Thing> EnumerateThings()
        {
            var list = YodiwoNestThings.Values;
            return list;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public IEnumerable<ThingType> EnumerateThingTypes()
        {
            var stypes = new HashSet<string>();
            foreach (var thing in YodiwoNestThings.Values)
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
        //------------------------------------------------------------------------------------------------------------------------
        public void SetThingsState(PortKey portKey, string state)
        {
            var devid = portKey.ThingKey.ThingUID;
            NestDescriptor nestdesc = null;
            if (!nestdescriptors.TryGetValue(devid, out nestdesc))
                DebugEx.TraceError("Not such devid:" + devid);
            else
            {
                var portUID = portKey.PortUID;
                setValue(nestdesc.type.ToStringInvariant(), devid, state, portUID);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public IEnumerable<Yodiwo.API.Plegma.Thing> Scan(ThingKey key)
        {
            return null;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool Delete(Thing[] incomingThings)
        {
            return false;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void OnPortEvent(PortEventMsg msg)
        {
        }
        //------------------------------------------------------------------------------------------------------------------------
        private static async void setValue(string type, string id, string value, string attribute)
        {
            try
            {
                using (HttpClient http = new HttpClient())
                {
                    string url = "https://developer-api.nest.com/";
                    StringContent content = null;
                    url += "devices/" + type.ToLowerInvariant() + "/" + id.Replace("_", "-") + "/?auth=" + NestModule.accessToken;
                    content = new StringContent("{\"" + attribute + "\":" + value + "}");
                    HttpResponseMessage rep = await http.PutAsync(new Uri(url), content);
                    if (null != rep)
                    {
                        DebugEx.TraceLog("http.PutAsync2=" + rep.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
    }
}
