using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.mNode.Plugins.UI.ContextMenu;
using FliclibDotNetClient;
using Yodiwo.mNode.Plugins.UI.Forms;
using Yodiwo.mNode.Plugins.UI.Forms.Controls;
using System.Threading;

namespace Yodiwo.mNode.Plugins.Integration_Flic
{
    public class PluginMain : Plugin
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        FlicClient _flicClient = null;
        //------------------------------------------------------------------------------------------------------------------------
        [NonSerialized]
        DictionaryTS<Bdaddr, ButtonConnectionChannel> ButtonChannels = new DictionaryTS<Bdaddr, ButtonConnectionChannel>();

        [NonSerialized]
        DictionaryTS<Bdaddr, Thing> flicThings = new DictionaryTS<Bdaddr, Thing>();
        private YConfig<FlicPluginConfig> YConfig;
        public static FlicPluginConfig ActiveCfg;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Initialize(mNodeConfig mNodeConfig, string PluginConfig, string UserConfig)
        {
            YConfig = Config.Init(PluginWorkingDirectory);
            ActiveCfg = YConfig.GetActiveConf();

            //init base
            if (base.Initialize(mNodeConfig, PluginConfig, UserConfig) == false)
                return false;

            //init
            try
            {
                DebugEx.TraceLog("FLIC: InitFlicClient from flic plugin initialize");
                //init plugin
                _InitFlicClient();

            }
            catch (Exception ex) { DebugEx.Assert(ex, "Could not init"); }
            //done
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Deinitialize()
        {
            try
            {
                base.Deinitialize();

                //deinit client
                try { DeInitFlicClient(); } catch { }
                return true;
            }
            catch { return false; }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void OnTransportConnected(string msg)
        {
            base.OnTransportConnected(msg);
            //init client if not already initialized
            // _InitFlicClient();
        }

        #region Flic server handler

        private void _InitFlicClient()
        {
            if (_flicClient == null)
            {
                DebugEx.TraceLog("FlicClient is null, Find it!!!!");
                while (true)
                {
                    try
                    {
                        _flicClient = FlicClient.Create(ActiveCfg.FlicDaemonHostname, ActiveCfg.FlicDaemonPort);
                        DebugEx.TraceLog("Connection to FlicClient successful");
                        if (_flicClient != null)
                        {
                            _flicClient.GetInfo(
                            (bluetoothControllerState, myBdAddr, myBdAddrType, maxPendingConnections, maxConcurrentlyConnectedButtons,
                            currentPendingConnections, currentlyNoSpaceForNewConnection, verifiedButtons) =>
                            {
                                DebugEx.TraceLog("Bluetooth controller status: " + bluetoothControllerState.ToString());
                                foreach (var bdAddr in verifiedButtons)
                                {
                                    GotButton(bdAddr);
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugEx.TraceErrorException(ex);
                        Thread.Sleep(500);
                        continue;
                    }
                    break;
                }

                _flicClient.BluetoothControllerStateChange += (o, args) =>
                {
                    DebugEx.TraceLog("Bluetooth controller status: " + args.State.ToString());
                };

                _flicClient.NewVerifiedButton += (o, args) =>
                {
                    DebugEx.TraceLog("new NewVerifiedButtonEvent");
                    GotButton(args.BdAddr);
                };

                TaskEx.RunSafe(() =>
                {
                    _flicClient.HandleEvents(); // HandleEvents returns when the socket has disconnected for any reason
                    DebugEx.TraceError("BLE connection lost");
                    OnBLEDisconnected();
                });
            }
        }

        private void OnBLEDisconnected()
        {
            try
            {
                foreach (var chan in ButtonChannels.Values)
                    _flicClient.RemoveConnectionChannel(chan);

                _flicClient.Disconnect();
                _flicClient = null;

                DebugEx.TraceLog("FLIC: InitFlicClient from OnBLEDisconnected");
                _InitFlicClient();
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex);
            }
        }

        private void DeInitFlicClient()
        {
            foreach (var chan in ButtonChannels.Values)
                _flicClient.RemoveConnectionChannel(chan);

            _flicClient.Disconnect();
            _flicClient = null;

            ButtonChannels.Clear();
            flicThings.Clear();
        }

        private void GotButton(Bdaddr bdAddr)
        {
            DebugEx.TraceLog("Got Button");
            var thing = flicThings.TryGetOrDefault(bdAddr);

            if (thing == null)
            {
                thing = ThingTools.FlicThing.CreateThing(bdAddr.ToString().Replace(":", ""), bdAddr.ToString());
                thing = AddThing(thing);
                flicThings.Add(bdAddr, thing);
            }

            DebugEx.TraceLog("===========>Add Button Thing Completed");
            var channel = ButtonChannels.TryGetOrDefault(bdAddr);
            if (channel == null)
            {
                DebugEx.TraceLog("===========>New Channel is created");
                channel = new ButtonConnectionChannel(bdAddr);

                channel.CreateConnectionChannelResponse += (sender1, eventArgs) =>
                {
                    if (eventArgs.Error == CreateConnectionChannelError.NoError)
                    {
                        _channelConnected((ButtonConnectionChannel)sender1);
                    }
                    else
                    {
                        DebugEx.TraceError(((ButtonConnectionChannel)sender1).BdAddr.ToString() + " could not be connected");
                    }
                };
                channel.Removed += (sender1, eventArgs) =>
                {
                    _channelDisconnected((ButtonConnectionChannel)sender1);
                };
                channel.ConnectionStatusChanged += (sender1, eventArgs) =>
                {
                    var chan = (ButtonConnectionChannel)sender1;
                    if (eventArgs.ConnectionStatus == ConnectionStatus.Disconnected)
                        _channelDisconnected(chan);
                };
                channel.ButtonSingleOrDoubleClickOrHold += (sender1, eventArgs) =>
                {
                    var chan = (ButtonConnectionChannel)sender1;
                    var thisThing = flicThings.TryGetOrDefault(chan.BdAddr);
                    if (thisThing == null)
                        return;

                    DebugEx.TraceLog(eventArgs.ClickType + " for " + thisThing.Name + " (key:" + thisThing.ThingKey + ")");
                    switch (eventArgs.ClickType)
                    {
                        case ClickType.ButtonSingleClick:
                            SetPortState(PortKey.BuildFromArbitraryString(thisThing.ThingKey, ThingTools.FlicThing.SingleClick), "True");
                            break;
                        case ClickType.ButtonDoubleClick:
                            SetPortState(PortKey.BuildFromArbitraryString(thisThing.ThingKey, ThingTools.FlicThing.DoubleClick), "True");
                            break;
                        case ClickType.ButtonHold:
                            SetPortState(PortKey.BuildFromArbitraryString(thisThing.ThingKey, ThingTools.FlicThing.LongClick), "True");
                            break;
                        default:
                            break;
                    }

                    if (!ButtonChannels.ContainsKey(chan.BdAddr))
                        _channelConnected(chan);
                };

                //try to add channel
                _flicClient.AddConnectionChannel(channel);
                DebugEx.TraceLog("===========>Add Connection Channel Completed");

                foreach (var navctx in NavigationContext.Values)
                {
                    DebugEx.TraceLog("===========>navctx.CurrentPage.Title: " + navctx.CurrentPage.Title);
                    if (navctx.CurrentPage.Title == "Flic Pairing")
                    {
                        DebugEx.TraceLog("=====>here I am <======");
                        navctx.GoBack();
                        navctx.UpdateCurrentPage(createDiscoverPage());
                    }
                }
            }
        }

        private void _channelDisconnected(ButtonConnectionChannel channel)
        {
            var bdAddr = channel.BdAddr;
            DebugEx.TraceLog(channel.ToString() + " disconnected");

            ButtonChannels.Remove(bdAddr);
        }
        private void _channelConnected(ButtonConnectionChannel channel)
        {
            var bdAddr = channel.BdAddr;
            DebugEx.TraceLog(channel.ToString() + " connected");

            ButtonChannels.ForceAdd(bdAddr, channel);
        }

        #region GUI

        public override void UI_mNodeForms_OnNewNavigationContext(NavigationContext navCtx, string StartPageUri)
        {
            base.UI_mNodeForms_OnNewNavigationContext(navCtx, StartPageUri);
            //navigate to start page
            navCtx.NavigateTo(createDiscoverPage());
        }

        public Page createDiscoverPage()
        {
            //create main page
            //onvifs
            Page page = new Page()
            {
                Uri = "main",
                Name = "main",
                Title = "Main page",
                Controls = new ListTS<Control>()
                {
                    new UI.Forms.Controls.StackPanel()
                    {
                        Items = new ListTS<Control>()
                        {
                             new UI.Forms.Controls.Button()
                             {
                                 Name = "add",
                                 Text = "Add new Flic(s)",
                                 Clicked = btnAdd_Clicked,
                             },
                            new UI.Forms.Controls.Button()
                             {
                                 Name = "remove",
                                 Text = "Remove All Flic(s)",
                                 Clicked = btnRemoveAll_Clicked,
                             },
                        }
                    }
                },
            };

            if (flicThings.Any())
            {
                var stack = page.Controls[0] as StackPanel;
                stack?.Items.Add(
                    new UI.Forms.Controls.Label()
                    {
                        Text = "Existing Flic Button(s)",
                    });

                foreach (var flic in flicThings)
                {
                    //var uri = new Uri("BdAsrress" + flic.Key.ToString().Replace(":", ""));
                    stack?.Items.Add(
                        new UI.Forms.Controls.Button()
                        {
                            Tag = flic.Key.ToString(),
                            Name = "config-" + flic.Key,
                            Text = flic.Key.ToString(),
                            Clicked = btn_Clicked
                        });
                }
            }
            return page;
        }

        private void btn_Clicked(NavigationContext ctx, Button control, UIEvent args)
        {
            ctx.NavigateTo(RemoveButtonPage(control?.TagStr));
        }

        private void btnRemove_Clicked(NavigationContext ctx, Button control, UIEvent args)
        {
            var bdaddr = control?.TagStr;
            DebugEx.TraceLog("BDAddr: " + bdaddr);
            //delete things from cloud
            flicThings.ForEach(f => { if (f.Key.ToString() == bdaddr) { try { DeleteThing(f.Value.ThingKey); } catch (Exception ex) { DebugEx.Assert(ex); } } });
            //delete specific flicThing from local Dictionary
            flicThings.RemoveWhere(x => x.Key.ToString() == bdaddr);
            //flicd will be closed, hence all channels should be recreated
            ButtonChannels.Clear();
            DebugEx.TraceLog("=======>" + ActiveCfg.PythonServer + ActiveCfg.restrouteremoveflic + "<=========");
            Dictionary<string, string> data = new Dictionary<string, string>() { { "bdaddr", bdaddr } };
            var rsp = Yodiwo.Tools.Http.RequestGET(ActiveCfg.PythonServer + ActiveCfg.restrouteremoveflic, data, null);
            DebugEx.TraceLog("=======>Single Flic Removal<======" + rsp.ResponseBodyText);
            //return to mainpage
            ctx.UpdateCurrentPage(createDiscoverPage());
        }

        private void btnRemoveAll_Clicked(NavigationContext ctx, Button control, UIEvent args)
        {
            if (flicThings.Any())
            {
                //delete things from cloud
                flicThings.ForEach(f => { try { DeleteThing(f.Value.ThingKey); } catch (Exception ex) { DebugEx.Assert(ex); } });
                //empty local dictionary
                flicThings.Clear();
                //flicd will be closed, hence all channels should be recreated
                ButtonChannels.Clear();
                DebugEx.TraceLog("=======>" + ActiveCfg.PythonServer + ActiveCfg.restrouteremoveflic + "<=========");
                Dictionary<string, string> data = new Dictionary<string, string>() { { "bdaddr", "" } };
                var rsp = Yodiwo.Tools.Http.RequestGET(ActiveCfg.PythonServer + ActiveCfg.restrouteremoveflic);
                DebugEx.TraceLog("=======>Global Flics Removal<======" + rsp.ResponseBodyText);
            }
            //return to mainpage
            ctx.UpdateCurrentPage(createDiscoverPage());

        }

        void btnAdd_Clicked(NavigationContext ctx, Button control, UIEvent args)
        {
            AddNewFlic();
            ctx.NavigateTo(AddButtonPage());
        }

        private Page AddButtonPage()
        {
            return new Page()
            {
                Uri = "Add Flic",
                Name = "Add Flic",
                Title = "Add Flic",
                Controls = new ListTS<Control>()
                {
                    new UI.Forms.Controls.StackPanel()
                    {
                        Items = new ListTS<Control>()
                        {
                             new UI.Forms.Controls.Label()
                             {
                                 Text= "Press Your Flic Button",
                             },
                        }
                    }
                }
            };
        }

        private Page RemoveButtonPage(string bdaddr)
        {
            DebugEx.TraceLog("Remove Button Page:" + bdaddr);
            return new Page()
            {
                Uri = "Remove Flic",
                Name = "Remove Flic",
                Title = "Remove Flic",
                Controls = new ListTS<Control>()
                {
                    new UI.Forms.Controls.StackPanel()
                    {
                        Items = new ListTS<Control>()
                        {
                             new UI.Forms.Controls.Label()
                             {
                                 Text= "Remove FlicButton",
                             },
                              new UI.Forms.Controls.Button()
                        {
                            Tag = bdaddr.ToString(),
                            Name = "config-" + bdaddr,
                            Text = bdaddr.ToString(),
                            Clicked = btnRemove_Clicked
                        }
                        }
                    }
                }
            };
        }

        private void AddNewFlic()
        {
            var scanWizard = new ScanWizard();
            scanWizard.FoundPrivateButton += ScanWizard_FoundPrivateButton;
            scanWizard.FoundPublicButton += ScanWizard_FoundPublicButton;
            scanWizard.ButtonConnected += ScanWizard_ButtonConnected;
            scanWizard.Completed += ScanWizard_Completed;
            _flicClient.AddScanWizard(scanWizard);
        }
        private Page PairFlicButtonPage()
        {

            return new Page()
            {
                Uri = "Flic Pairing",
                Name = "Flic Pairing",
                Title = "Flic Pairing",
                Controls = new ListTS<Control>()
                {
                    new UI.Forms.Controls.StackPanel()
                    {
                        Items = new ListTS<Control>()
                        {
                             new UI.Forms.Controls.Label()
                             {
                                 Text= "Hold down your Flic button for 7 seconds, then Press Main Page",
                             },
                        }
                    }
                }
            };
        }

        private void ScanWizard_Completed(object sender, ScanWizardCompletedEventArgs e)
        {
            DebugEx.TraceLog("Result: " + e.Result);
            if (e.Result != ScanWizardResult.WizardSuccess)
                foreach (var navctx in NavigationContext.Values)
                {

                    if (navctx.CurrentPage.Title == "Flic Pairing")
                        navctx.UpdateCurrentPage(createDiscoverPage());
                }

        }

        private void ScanWizard_ButtonConnected(object sender, ScanWizardButtonInfoEventArgs e)
        {
            DebugEx.TraceLog("Connected to " + e.BdAddr.ToString() + ", now pairing...");
        }

        private void ScanWizard_FoundPublicButton(object sender, ScanWizardButtonInfoEventArgs e)
        {
            DebugEx.TraceLog("Found button " + e.BdAddr.ToString() + ", now connecting...");

        }

        private void ScanWizard_FoundPrivateButton(object sender, EventArgs e)
        {
            DebugEx.TraceLog("ScanWizard_FoundPrivateButton");
            foreach (var navctx in NavigationContext.Values)
            {
                if (navctx.CurrentPage.Title == "Add Flic")
                    navctx.UpdateCurrentPage(PairFlicButtonPage());
            }
        }
        #endregion

        #endregion

        #endregion
    }
}