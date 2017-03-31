using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yodiwo;
using Yodiwo.API.Plegma;
using Yodiwo.mNode.Plugins.UI.ContextMenu;
using Yodiwo.mNode.Plugins.UI.Forms;
using Yodiwo.mNode.Plugins.UI.Forms.Controls;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Yodiwo.mNode.Plugins.DevelopmentTools
{
    public class PluginMain : Plugin
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        PluginConfig Config;
        bool _CloudConnected = false;
        //------------------------------------------------------------------------------------------------------------------------
        // Periodic thing events
        bool PeriodicRunning = false;
        Task PeriodicTask = null;
        int Period = 10000;
        //------------------------------------------------------------------------------------------------------------------------
        DictionaryTS<string, Thing> things = new DictionaryTS<string, Thing>();
        // Key: generic string
        DictionaryTS<string, Port> Ports = new DictionaryTS<string, Port>();
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Initialize(mNodeConfig mNodeConfig, string PluginConfig, string UserConfig)
        {
            //init base
            if (base.Initialize(mNodeConfig, PluginConfig, UserConfig) == false)
                return false;

            //read config
            if (!string.IsNullOrWhiteSpace(PluginConfig))
                try { Config = (PluginConfig ?? "").FromJSON<PluginConfig>(); } catch { Config = new PluginConfig(); }


            //init
            try
            {
                SetupSerialPortThing();

                DebugEx.TraceLog("Successful start of DevelopmentTools plugin.");
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Could not init"); }

            //done
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void StoreConfig()
        {
            lock (Config)
                UpdateConfig(Config?.ToJSON() ?? "");
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Deinitialize()
        {
            try
            {
                base.Deinitialize();


                return true;
            }
            catch { return false; }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnTransportConnected(string msg)
        {
            base.OnTransportConnected(msg);
            _CloudConnected = true;


            // Send the connection timestamp
            TaskEx.RunSafe(() =>
            {
                // Send the report 10Second after connection to avoid the timing issue with active port key
                // TODO: Fix this case !!!!
                Thread.Sleep(10000);
                if (Ports?.ContainsKey("connectionTimestamp") ?? false)
                {
                    SetPortState(new List<TupleS<string, string>>()
                                    {
                                        new TupleS<string, string>(Ports["connectionTimestamp"].PortKey, DateTime.UtcNow.ToStringInvariant()),
                                        new TupleS<string, string>(Ports["isTransportConnected"].PortKey, "True"),
                                    });
                }
            });
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnTransportDisconnected(string msg)
        {
            base.OnTransportDisconnected(msg);
            _CloudConnected = false;
        }
        //------------------------------------------------------------------------------------------------------------------------
        bool GetValue<T>(MatchCollection mc, string groupName, out T value)
        {
            value = default(T);
            foreach (Match m in mc)
            {
                var group = m.Groups[groupName];
                if (group != null)
                {
                    try
                    {
                        value = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(group.Value);
                        //DebugEx.TraceLog($"Value of {groupName}:{value}");
                        return true;
                    }
                    catch
                    {
                        //oops?
                        DebugEx.TraceLog($"Could not parse match {group.Value}");
                    }
                }
            }
            return false;
        }
        //------------------------------------------------------------------------------------------------------------------------


        #region Yodiwo Serial Port Node specific methods
        //------------------------------------------------------------------------------------------------------------------------
        public static string CreateThingKey(string bridgeID)
        {
            return ThingKeyPrefix + "Development_" + bridgeID;
        }
        //------------------------------------------------------------------------------------------------------------------------
        private IEnumerable<Thing> SetupSerialPortThing()
        {
            // Clean old things
            things.Clear();

            // mNode transport conncted event
            {
                var isTransportConnected = new Port()
                {
                    ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                    Name = "Connected",
                    State = "",
                    ConfFlags = ePortConf.IsTrigger,
                    Type = Yodiwo.API.Plegma.ePortType.Boolean,
                    PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                };
                Ports.Add("isTransportConnected", isTransportConnected);

                var connectionTimestamp = new Port()
                {
                    ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                    Name = "Timestamp",
                    State = "",
                    ConfFlags = ePortConf.None,
                    Type = Yodiwo.API.Plegma.ePortType.Timestamp,
                    PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "1")
                };
                Ports.Add("connectionTimestamp", connectionTimestamp);

                var t = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", CreateThingKey("0")),
                    Type = "",
                    Name = "Transport",
                    Config = new List<ConfigParameter>(),
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "http://simpleicon.com/wp-content/uploads/cloud-connection-1.png",
                    },

                    Ports = new List<Port>()
                    {
                        isTransportConnected,
                        connectionTimestamp
                    }
                };

                t = AddThing(t);
                things.Add("Transport", t);
            }

            // mNode periodic send
            {
                var periodicTimestamp = new Port()
                {
                    ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                    Name = "Timestamp",
                    State = "",
                    ConfFlags = ePortConf.None,
                    Type = Yodiwo.API.Plegma.ePortType.Timestamp,
                    PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                };
                Ports.Add("periodicTimestamp", periodicTimestamp);

                var t = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", CreateThingKey("1")),
                    Type = "",
                    Name = "Periodic",
                    Config = new List<ConfigParameter>()
                {
                    // TODO: Use the actual value of the thing
                    new ConfigParameter() {
                        Description = "Period Seconds",
                        Name        = "Period",
                        Value       = "10"
                    }
                },
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "https://openreferral.org/wp-content/uploads/2015/02/icon_4034-300x300.png",
                    },

                    Ports = new List<Port>()
                    {
                        periodicTimestamp
                    }
                };

                t = AddThing(t);
                things.Add("Periodic", t);
            }

            // mNode Ping
            {
                var echoPortOut = new Port()
                {
                    ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                    Name = "Echo",
                    State = "",
                    ConfFlags = ePortConf.None,
                    Type = Yodiwo.API.Plegma.ePortType.String,
                    PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                };
                Ports.Add("echoPortOut", echoPortOut);

                var echoPortIn = new Port()
                {
                    ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                    Name = "Echo",
                    State = "",
                    ConfFlags = ePortConf.None,
                    Type = Yodiwo.API.Plegma.ePortType.String,
                    PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "1")
                };
                Ports.Add("echoPortIn", echoPortIn);

                var t = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", CreateThingKey("2")),
                    Type = "",
                    Name = "Ping",
                    Config = new List<ConfigParameter>(),
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "https://apk-dl.com/detail/image/com.lipinic.ping-w250.png",
                    },

                    Ports = new List<Port>()
                    {
                        echoPortIn,
                        echoPortOut
                    }
                };

                t = AddThing(t);
                things.Add("Ping", t);
            }

            return things.Values;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnPortEvent(PortEventMsg msg)
        {
            foreach (var pe in msg.PortEvents)
            {
                try
                {
                    // Echo state
                    if (Ports["echoPortIn"].PortKey == pe.PortKey)
                    {
                        SetPortState(Ports["echoPortOut"].PortKey, pe.State);
                    }
                }
                catch (Exception ex)
                {
                    DebugEx.TraceErrorException(ex);
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnThingActivated(ThingKey thingKey)
        {
            DebugEx.TraceLog($"Thing Activated {thingKey}");

            // Check when we activate periodic
            if (things["Periodic"]?.ThingKey == thingKey)
            {
                DebugEx.TraceLog("Periodic Thing Activated");

                // Dispose old task
                PeriodicRunning = false;
                PeriodicTask?.Wait();
                PeriodicTask?.Dispose();

                // Create a new cancellation source
                PeriodicRunning = true;

                //start alive task
                PeriodicTask = TaskEx.RunSafe(() =>
                {
                    while (!IsDisposed && PeriodicRunning)
                    {
                        // Send timestamp if we are connected
                        if (_CloudConnected &&
                            (Ports?.ContainsKey("periodicTimestamp") ?? false))
                        {
                            SetPortState(Ports["periodicTimestamp"].PortKey, DateTime.UtcNow.ToStringInvariant());
                        }
                        Thread.Sleep(Period);
                    }
                });
            }


        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnThingDeactivated(ThingKey thingKey)
        {
            // Check when we activate periodic
            if (things["Periodic"]?.ThingKey == thingKey)
            {
                DebugEx.TraceLog("Periodic Thing Deactivated");

                // Dispose old task
                PeriodicRunning = false;
                PeriodicTask?.Wait();
                PeriodicTask?.Dispose();
                PeriodicTask = null;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #endregion
    }

}

