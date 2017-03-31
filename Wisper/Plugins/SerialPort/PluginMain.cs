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

namespace Yodiwo.mNode.Plugins.Bridge_SerialPort
{
    public class PluginMain : Plugin
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        Thread _SerialPortReadThread;
        SerialPort mySerial;
        bool _IsRunning;
        //------------------------------------------------------------------------------------------------------------------------
        static string SerialStatusReq = "ATI001$0D";
        //------------------------------------------------------------------------------------------------------------------------
        Port ReceiveFromCloudPort;
        Port SendToCloudPort;
        //------------------------------------------------------------------------------------------------------------------------
        RequestQueueConsumer<string> SendQueue = new RequestQueueConsumer<string>();
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Initialize(mNodeConfig mNodeConfig, string PluginConfig, string UserConfig)
        {
            //init base
            if (base.Initialize(mNodeConfig, PluginConfig, UserConfig) == false)
                return false;

            //set queue handler
            SendQueue.SetHandler(s =>
            {
                if (SendToCloudPort != null)
                    SetPortState(SendToCloudPort.PortKey, s);
            });

            //init
            try
            {
                var ports = SerialPort.GetPortNames();
                DebugEx.TraceLog("Serial Ports found: " + string.Join(",", ports));

                var port = ports?.Where(p => p.Contains("USB")).FirstOrDefault();
                if (port != null && !port.IsNullOrEmpty())
                {
                    DebugEx.TraceLog("Selected Port: " + port);

                    //replace this with entry from conf
                    mySerial = new SerialPort(port, 9600);
                    if (mySerial.IsOpen)
                    {
                        try
                        {
                            mySerial.Close();
                            mySerial.Dispose();
                        }
                        catch { }
                    }
                    mySerial.DataReceived += MySerial_DataReceived;
                    mySerial.ErrorReceived += MySerial_ErrorReceived;
                    mySerial.ReadTimeout = 2000;
                    mySerial.NewLine = "\r\n";
                    mySerial.Open();

                    _IsRunning = true;
                    StartSerialPortReadThread();

                    SendQueue.Start();
                    SendQueue.Pause();
                }
                else
                {
                    DebugEx.Assert("The USB port does not exist");
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Could not init"); }

            //setup
            SetupSerialPortThings();

            //done
            return true;
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void MySerial_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            DebugEx.TraceLog($"{sender} sent {e.EventType}");
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void MySerial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DebugEx.TraceLog($"{sender} sent {e.EventType}");
        }

        public override bool Deinitialize()
        {
            try
            {
                base.Deinitialize();

                _IsRunning = false;

                try
                {
                    if (!_SerialPortReadThread.Join(1000))
                        _SerialPortReadThread.Abort();
                }
                finally
                {
                    _SerialPortReadThread = null;
                }
                return true;
            }
            catch { return false; }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnTransportConnected(string msg)
        {
            base.OnTransportConnected(msg);
            SendQueue.Continue();
            DebugEx.TraceLog("Transport connected");
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnTransportDisconnected(string msg)
        {
            base.OnTransportDisconnected(msg);
            SendQueue.Pause();
            DebugEx.TraceLog("Transport disconnected");
        }
        //------------------------------------------------------------------------------------------------------------------------

        private void StartSerialPortReadThread()
        {
            _SerialPortReadThread = new Thread(() =>
            {
                string line;

                DebugEx.TraceLog($"Serial reading thread started in Port:{mySerial.PortName}, IsOpen:{mySerial.IsOpen}");

                while (_IsRunning)
                {
                    try
                    {
                        line = mySerial.ReadLine();

                        SendQueue.Enqueue(line);
                        DebugEx.TraceLog($"data read: \"{line}\"");
                    }
                    catch (TimeoutException tout_ex) { }
                    catch (InvalidOperationException inv_ex)
                    {
                        DebugEx.TraceErrorException(inv_ex, "InvalidOperationException");
                    }
                    catch (Exception ex)
                    {
                        DebugEx.TraceErrorException(ex, "GenericException");
                    }
                }
            });
            _SerialPortReadThread.IsBackground = true;
            _SerialPortReadThread.Start();
        }


        #region Yodiwo Serial Port Node specific methods
        //------------------------------------------------------------------------------------------------------------------------
        public static string CreateThingId(string bridgeID)
        {
            return ThingKeyPrefix + "Serial:" + bridgeID;
        }
        //------------------------------------------------------------------------------------------------------------------------
        private List<Thing> SetupSerialPortThings()
        {
            List<Thing> things = new List<Thing>();

            // Serial Port Data
            {
                var tkey1 = new ThingKey(NodeKey, CreateThingId("1"));
                SendToCloudPort = new Port()
                {
                    ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                    Name = "FromSerialPort",
                    State = "",
                    ConfFlags = ePortConf.None,
                    Type = Yodiwo.API.Plegma.ePortType.String,
                    PortKey = PortKey.BuildFromArbitraryString(tkey1, "0")
                };

                var t = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = tkey1,
                    Type = "com.yodiwo.serialport",
                    Name = "SerialOut",
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "http://www.aimtouch.com/images/RS232-icon.png",
                    },

                    Ports = new List<Port>()
                    {
                        SendToCloudPort
                    }
                };

                t = AddThing(t);
                things.Add(t);
            }

            // Serial Port success write
            {
                var tkey2 = new ThingKey(NodeKey, CreateThingId("2"));
                ReceiveFromCloudPort = new Port()
                {
                    ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                    Name = "ToSerialPort",
                    State = "",
                    ConfFlags = ePortConf.None,
                    Type = Yodiwo.API.Plegma.ePortType.String,
                    PortKey = PortKey.BuildFromArbitraryString(tkey2, "0")
                };

                var t = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = tkey2,
                    Type = "com.yodiwo.serialport",
                    Name = "SerialIn",
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "http://www.aimtouch.com/images/RS232-icon.png",
                    },

                    Ports = new List<Port>()
                    {
                        ReceiveFromCloudPort
                    }
                };

                t = AddThing(t);
                things.Add(t);
            }

            return things;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnPortEvent(PortEventMsg msg)
        {
            foreach (var pe in msg.PortEvents)
            {
                try
                {
                    DebugEx.TraceLog("fromCloud: " + pe.PortKey + "  " + pe.State);

                    if (ReceiveFromCloudPort.PortKey == pe.PortKey)
                    {
                        //WRITE to serial
                        mySerial.WriteLine(pe.State);
                    }
                }
                catch (Exception ex)
                {
                    DebugEx.TraceErrorException(ex);
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #endregion
    }

}

