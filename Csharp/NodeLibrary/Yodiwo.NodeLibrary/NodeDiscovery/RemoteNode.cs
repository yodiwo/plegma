using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.YPChannel;

namespace Yodiwo.NodeLibrary.NodeDiscovery
{
    public class RemoteNode : IDisposable
    {
        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public readonly Node Node;
        //-------------------------------------------------------------------------------------------------------------------------
        public readonly NodeDiscoverManager NodeDiscoverManager;
        //-------------------------------------------------------------------------------------------------------------------------
        public DiscoveryMessage DiscoveryMessage;
        public YPChannel.Transport.Sockets.LANDiscoverer.RemoteEndpointID RemoteEndpointID;
        public string IPAddress => RemoteEndpointID.IPAddress;
        public int RemotePort => DiscoveryMessage?.YPChannelPort ?? 0;
        //-------------------------------------------------------------------------------------------------------------------------
        public NodeKey RemoteNodeKey;
        //-------------------------------------------------------------------------------------------------------------------------
        public YPChannel.Transport.Sockets.Client ClientChannel;
        public YPChannel.Transport.Sockets.ServerChannel ServerChannel;
        //-------------------------------------------------------------------------------------------------------------------------
        public bool IsConnected => ((ClientChannel?.IsOpen ?? false) || (ServerChannel?.IsOpen ?? false));
        //-------------------------------------------------------------------------------------------------------------------------
        byte[] PublicKey;
        //-------------------------------------------------------------------------------------------------------------------------
        DateTime lastConnectionAttempt = DateTime.Now;
        static readonly TimeSpan reconnectionAttemptSleep = TimeSpan.FromSeconds(10);
        public bool ReconnectionPeriodElapsed => DateTime.Now - lastConnectionAttempt < reconnectionAttemptSleep;
        //-------------------------------------------------------------------------------------------------------------------------
        public delegate void OnChannelOpenDelegate(RemoteNode RemoteNode);
        public event OnChannelOpenDelegate OnChannelOpen = null;

        public delegate void OnChannelCloseDelegate(RemoteNode RemoteNode);
        public event OnChannelCloseDelegate OnChannelClose = null;

        public delegate void OnVBMReceivedDelegate(RemoteNode RemoteNode, VirtualBlockEventMsg msg);
        public event OnVBMReceivedDelegate OnVBMReceived = null;
        //-------------------------------------------------------------------------------------------------------------------------
        bool _IsDisposed = false;
        public bool IsDisposed => _IsDisposed;
        //-------------------------------------------------------------------------------------------------------------------------
        public bool ConnectionTaskRunning;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constuctor
        //-------------------------------------------------------------------------------------------------------------------------
        public RemoteNode(Node Node, NodeDiscoverManager NodeDiscoverManager)
        {
            //keep
            this.Node = Node;
            this.NodeDiscoverManager = NodeDiscoverManager;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //-------------------------------------------------------------------------------------------------------------------------
        public void StartConnectionTask()
        {
            //already running?
            if (ConnectionTaskRunning)
                return;

            //start new task
            Task.Run(async () =>
            {
                ConnectionTaskRunning = true;
                try
                {

                    while (!_IsDisposed && ClientChannel?.State != YPChannel.ChannelStates.Open)
                    {
                        if (Connect())
                            break;
                        else
                            await Task.Delay(reconnectionAttemptSleep);
                    }
                }
                catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception in connection Task"); }
                finally { ConnectionTaskRunning = false; }
            });
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public bool Connect()
        {
            YPChannel.Transport.Sockets.Client _client = null;
            lock (this)
                try
                {
                    //check that is not already connecting/conntected
                    var _tmpypc = ClientChannel;
                    if (_tmpypc != null && _tmpypc.State != YPChannel.ChannelStates.Closed)
                        return _tmpypc.State == YPChannel.ChannelStates.Open;

                    //dispose previous ypchannel
                    if (_tmpypc != null)
                    {
                        try { _tmpypc.Close("Resetting channel"); } catch { }
                        try { _tmpypc.Dispose(); } catch { }
                    }

                    //create ypchannel client
                    _client = new YPChannel.Transport.Sockets.Client(NodeDiscoverManager.Protocol);
                    SetupChannel(_client);

                    //connect
                    DebugEx.TraceLog($"RemoteNode : Connecting to node. (ip:{IPAddress} port:{RemotePort} nodekey:{RemoteNodeKey})");
                    lastConnectionAttempt = DateTime.Now;
                    var conRes = _client.Connect(IPAddress, RemotePort, false);
                    if (conRes)
                    {
                        DebugEx.TraceLog($"RemoteNode : Connected to node. (ip:{IPAddress} port:{RemotePort} nodekey:{RemoteNodeKey})");
                        return true;
                    }
                    else
                    {
                        try { _client.Close("error(1)"); } catch { }
                        try { _client.Dispose(); } catch { }
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    DebugEx.TraceError(ex, "Error while connecting to discovered node");

                    //close channel
                    try { _client?.Close("erro(2)r"); } catch { }
                    try { _client?.Dispose(); } catch { }

                    //switch state to reconnection sleep
                    lastConnectionAttempt = DateTime.Now;
                    return false;
                }
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public void SetupChannel(YPChannel.Channel _channel)
        {
            if (_channel == null)
            {
                DebugEx.Assert("null channel");
                return;
            }

            //check for previous channels
            if (_channel.ChannelRole == YPChannel.ChannelRole.Client && ClientChannel != null)
            {
                try { ClientChannel.Close("channel setup (1)"); } catch { }
                try { ClientChannel.Dispose(); } catch { }
                unhookChannel(ClientChannel);
                ClientChannel = null;
            }
            else if (_channel.ChannelRole == YPChannel.ChannelRole.Server && ServerChannel != null)
            {
                try { ServerChannel.Close("channel setup (2)"); } catch { }
                try { ServerChannel.Dispose(); } catch { }
                unhookChannel(ServerChannel);
                ServerChannel = null;
            }

            //keep new channel
            if (_channel is YPChannel.Transport.Sockets.ServerChannel)
                ServerChannel = _channel as YPChannel.Transport.Sockets.ServerChannel;
            else if (_channel is YPChannel.Transport.Sockets.Client)
                ClientChannel = _channel as YPChannel.Transport.Sockets.Client;
            else
                DebugEx.Assert("Should not be here");

            //hook channel events
            hookChannel(_channel);
        }
        //-------------------------------------------------------------------------------------------------------------------------
        void hookChannel(YPChannel.Channel _channel)
        {
            if (_channel == null)
                return;
            _channel.OnClosedEvent += YPChannel_OnClosedEvent;
            _channel.OnOpenEvent += YPChannel_OnOpenEvent;
            _channel.OnMessageReceived += YPChannel_OnMessageReceived;
            if (_channel.ChannelRole == YPChannel.ChannelRole.Server)
                _channel.NegotiationHandler = YPChannel_Negotiation;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        void unhookChannel(YPChannel.Channel _channel)
        {
            if (_channel == null)
                return;
            _channel.OnClosedEvent -= YPChannel_OnClosedEvent;
            _channel.OnOpenEvent -= YPChannel_OnOpenEvent;
            _channel.OnMessageReceived -= YPChannel_OnMessageReceived;
            if (_channel.ChannelRole == YPChannel.ChannelRole.Server)
                _channel.NegotiationHandler = null;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        private bool YPChannel_Negotiation(YPChannel.Channel Channel)
        {
            DebugEx.TraceLog($"RemoteNode : Negotiation started with node. (ip:{IPAddress} port:{RemotePort} nodekey:{RemoteNodeKey})");

            //check
            if (Channel.ChannelRole != YPChannel.ChannelRole.Server)
            {
                DebugEx.Assert("Should not be here");
                return false;
            }

            //---------------------
            // Association Request
            //---------------------
            {
                //send request
                var req = new AssociationRequest()
                {
                    UnsafeNodeKey = Node.NodeKey,
                    PublicKey = null,
                };
                var rsp = Channel.SendRequest<AssociationResponse>(req);
                if (rsp == null)
                    return false;

                //keep discovery message
                DiscoveryMessage = rsp.DiscoveryMessage;
                RemoteEndpointID.ID = rsp.DiscoveryMessage.Id;
                //examine discovery message
                //..

                //keep node key
                RemoteNodeKey = rsp.UnsafeNodeKey;
                if (RemoteNodeKey.IsInvalid)
                    return false;

                //keep public key
                /*
                if (rsp.PublicKey == null || rsp.PublicKey.Length == 0 || rsp.PublicKey.Length > 1024 * 20)
                    return false;
                else*/
                PublicKey = rsp.PublicKey;
            }

            //---------------------
            // Authentication Request
            //---------------------
            //..

            //all ok
            return true;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        private void YPChannel_OnOpenEvent(YPChannel.Channel Channel)
        {
            DebugEx.TraceLog($"RemoteNode : Openned channel with node. (ip:{IPAddress} port:{RemotePort} nodekey:{RemoteNodeKey})");
            //raise events
            OnChannelOpen?.Invoke(this);
        }
        //-------------------------------------------------------------------------------------------------------------------------
        private void YPChannel_OnClosedEvent(YPChannel.Channel Channel, string Message)
        {
            _disconnect();
            //raise events
            OnChannelClose?.Invoke(this);
        }
        //-------------------------------------------------------------------------------------------------------------------------
        private void YPChannel_OnMessageReceived(YPChannel.Channel Channel, YPChannel.YPMessage Message)
        {
            var _msg = Message.Payload;
            //handle negotiation
            if (_msg is AssociationRequest && Channel.ChannelRole == YPChannel.ChannelRole.Client)
            {
                DebugEx.TraceLog($"RemoteNode : Sending negotiation AssociationResponse. (ip:{IPAddress} port:{RemotePort} nodekey:{RemoteNodeKey})");
                var rsp = new AssociationResponse()
                {
                    DiscoveryMessage = NodeDiscoverManager.DiscoveryMessage,
                    UnsafeNodeKey = Node.NodeKey,
                    PublicKey = null,
                };
                Channel.SendResponseAsync(rsp, Message.MessageID);
            }
            else if (_msg is AuthenticationRequest && Channel.ChannelRole == YPChannel.ChannelRole.Client)
            {
                //...
            }
            else if (_msg is VirtualBlockEventMsg)
            {
                var msg = _msg as VirtualBlockEventMsg;
                var rsp = new GenericRsp() { IsSuccess = false };
                //validate msg
                if (msg.BlockEvents.Any(b => ((BlockKey)b.BlockKey).GraphKey.NodeKey != Node.NodeKey))
                {
                    DebugEx.Assert("Received VirtualBlockEventMsg that was not for me");
                    Channel.SendResponse(rsp, Message.MessageID);
                    return;
                }
                //respond with success
                rsp.IsSuccess = true;
                Channel.SendResponse(rsp, Message.MessageID);
                //raise event
                OnVBMReceived?.Invoke(this, msg);
            }
            else
                DebugEx.Assert("Unkown message received");
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public TResponse SendRequest<TResponse>(object Payload, TimeSpan? Timeout = null, TResponse Defaut = default(TResponse))
        {
            var _channel = ClientChannel != null && ClientChannel.IsOpen ? (Channel)ClientChannel : (Channel)ServerChannel;
            if (_channel != null && _channel.IsOpen)
                return _channel.SendRequest<TResponse>(Payload, Timeout: Timeout, Defaut: Defaut);
            else
                return Defaut;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public bool SendMessage<TResponse>(object Payload)
        {
            var _channel = ClientChannel != null && ClientChannel.IsOpen ? (Channel)ClientChannel : (Channel)ServerChannel;
            if (_channel != null && _channel.IsOpen)
            {
                _channel.SendMessage(Payload);
                return true;
            }
            else
                return false;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        internal void _disconnect()
        {
            try
            {
                //close client and channel (if server)
                try { ClientChannel?.Close("disconnecting"); unhookChannel(ClientChannel); ClientChannel?.Dispose(); } catch { }
                try { ServerChannel?.Close("disconnecting"); unhookChannel(ServerChannel); ServerChannel?.Dispose(); } catch { }

                //let go of references
                ClientChannel = null;
                ServerChannel = null;
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "Unhandled exception in disconnection"); }
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            try
            {
                lock (this)
                {
                    if (IsDisposed)
                        return;
                    else
                        _IsDisposed = true;

                    //disconnect
                    _disconnect();
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Dispose unhandled exception"); }
        }
        //-------------------------------------------------------------------------------------------------------------------------
        ~RemoteNode()
        {
            if (!_IsDisposed)
                try { Dispose(); } catch { }
        }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
