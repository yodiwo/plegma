using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.mNode.Plugins.CPP
{
    public class PluginMain : Plugin
    {
        #region Variables
        Thread CommandRetrieverThread;
        bool isRunning;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Initialize(mNodeConfig mNodeConfig, string PluginConfig, string UserConfig)
        {
            //init base
            if (base.Initialize(mNodeConfig, PluginConfig, UserConfig) == false)
                return false;


            //initialize native system (threads etc)
            try
            {
                if (Native.Initialize(ThingKeyPrefix) != 0)
                    DebugEx.TraceLog("Could not initialize native system");
                else
                {
                    DebugEx.TraceLog("Initialized native system");

                    // Get things
                    var json = Native.GetThings();
                    var things = json.FromJSON<List<Thing>>();
                    DebugEx.TraceLog($"Things:{things.Count} - {things[0].Name}");

                    // Fix keys
                    foreach (var t in things)
                    {
                        t.ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", t.ThingKey);
                        foreach (var p in t.Ports)
                        {
                            p.PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", p.PortKey);
                        }
                    }

                    // Add things
                    AddThing(things);


                    //start command retriver
                    isRunning = true;
                    CommandRetrieverThread = new Thread(commandRetrieverHeartbeatEntryPoint);
                    CommandRetrieverThread.IsBackground = true;
                    CommandRetrieverThread.Start();
                }
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "Failed to init plugin"); return false; }


            //init plugin
            DebugEx.TraceLog("C++11 Plugin up and running !! ");

            //done
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Deinitialize()
        {
            try
            {
                //deinint native
                var nativeRet = Native.Deinitialize() == 0;

                //stop thread
                isRunning = false;
                CommandRetrieverThread?.Join(1000);
                CommandRetrieverThread = null;

                return nativeRet;
            }
            catch (Exception ex)
            {
                DebugEx.TraceError(ex, "DeInitialize failed");
                return false;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        void commandRetrieverHeartbeatEntryPoint()
        {
            while (isRunning)
            {
                try
                {
                    var cmd = Native.GetMessage();
                    if (cmd != null)
                    {
                        try { processCommand(cmd); }
                        catch (Exception ex) { DebugEx.TraceError(ex, "processCommand failed"); }
                    }
                    else
                    {
                        DebugEx.TraceError("We get null message.");
                        Thread.Sleep(100); //something went wrong.. sleep for a while to avoid catastrophic spinning
                    }
                }
                catch (Exception ex)
                {
                    DebugEx.TraceError(ex, "Could not call .so native fuction");
                    return;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary> Process commands received by the native library </summary>
        void processCommand(string jsonMsg)
        {
            try
            {
                var msg = jsonMsg.FromJSON<Msg>();

                if (msg.Type == MsgType.SendPortEvent)
                {
                    var sendPortEvent = msg.Message.FromJSON<SendPortEvent>();
                    var tKey = ThingKey.BuildFromArbitraryString(this.NodeKey, sendPortEvent.ThingKey);
                    var pKey = PortKey.BuildFromArbitraryString(tKey, sendPortEvent.PortKey);
                    SetPortState(pKey, sendPortEvent.State);
                }

            }
            catch (Exception ex) { DebugEx.TraceError(ex, "processCommand failed"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        class PortEventExt : PortEvent
        {
            public string ThingUID;
            public string PortUID;
            public PortEventExt(PortEvent ev)
                : base(ev.PortKey, ev.State, ev.RevNum, ev.Timestamp)
            {
                PortKey p = new PortKey(ev.PortKey);
                this.ThingUID = p.ThingKey.ThingUID;
                this.PortUID = p.PortUID;
            }
        }
        public override void OnPortEvent(PortEventMsg msg)
        {
            try
            {
                var portEvents = msg.PortEvents.Select((x) => new PortEventExt(x)).ToList();
                Native.OnPortEvent(portEvents.ToJSON());
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

    }
}

