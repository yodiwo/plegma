using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.NodeLibrary.NodeDiscovery
{
    public class RemoteNode
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
        public enum RemoteNodeState
        {
            Unkown = 0,
            Connecting,
            Connected,
            Disconnected,
            Rejected,
        }
        public RemoteNodeState State = RemoteNodeState.Unkown;
        //-------------------------------------------------------------------------------------------------------------------------
        YPChannel.Transport.Sockets.Client _YPClient;
        public YPChannel.Channel Channel;
        //-------------------------------------------------------------------------------------------------------------------------
        public bool IsServer;
        public bool IsClient;
        //-------------------------------------------------------------------------------------------------------------------------
        public bool IsConnected => State == RemoteNodeState.Connected && (Channel?.IsOpen ?? false);
        //-------------------------------------------------------------------------------------------------------------------------
        byte[] PublicKey;
        //-------------------------------------------------------------------------------------------------------------------------
        DateTime lastConnectionAttempt = DateTime.Now;
        static readonly TimeSpan reconnectionAttemptSleep = TimeSpan.FromSeconds(30);
        public bool ReconnectionPeriodElapsed => DateTime.Now - lastConnectionAttempt < reconnectionAttemptSleep;
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
        //-------------------------------------------------------------------------------------------------------------------------
        public bool Connect(bool ignoreReconnectionPeriod = false)
        {
            lock (this)
            try
                {
                    //check reconnection period
                    if (!ignoreReconnectionPeriod && State == RemoteNodeState.Disconnected && !ReconnectionPeriodElapsed)
                        return false;

                    //check that is not already connecting
                    var _tmpypc = _YPClient;
                    if (State == RemoteNodeState.Connecting || (_tmpypc != null && _tmpypc.State != YPChannel.ChannelStates.Closed))
                        return false;

                    //create and setup ypchannel
                    _YPClient = new YPChannel.Transport.Sockets.Client(NodeDiscoverManager.Protocol);
                    SetupChannel(_YPClient);

                    //change state
                    State = RemoteNodeState.Connecting;

                    //connect
                    DebugEx.TraceLog($"RemoteNode : Connecting to node. (ip:{IPAddress} port:{RemotePort} nodekey:{RemoteNodeKey})");
                    lastConnectionAttempt = DateTime.Now;
                    var conRes = _YPClient.Connect(IPAddress, RemotePort, false);
                    if (conRes == false)
                    {
                        State = RemoteNodeState.Disconnected;
                        return false;
                    }
                    else
                    {
                        //change state
                        State = RemoteNodeState.Connected;
                        DebugEx.TraceLog($"RemoteNode : Connected to node. (ip:{IPAddress} port:{RemotePort} nodekey:{RemoteNodeKey})");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    DebugEx.TraceError(ex, "Error while connecting to discovered node");

                    //close channels
                    try
                    {
                        _YPClient?.Close();
                        _YPClient = null;
                    }
                    catch { }

                    //switch state to reconnection sleep
                    State = RemoteNodeState.Disconnected;
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
            if (this.Channel != null)
            {
                DebugEx.Assert("Trying to setup a channel on an existing channel");
                return;
            }
            //keep channel
            Channel = _channel;

            //setup events
            _channel.OnClosedEvent += YPChannel_OnClosedEvent;
            _channel.OnOpenEvent += YPChannel_OnOpenEvent;
            _channel.OnMessageReceived += YPChannel_OnMessageReceived;
            _channel.NegotiationHandler = YPChannel_Negotiation;

            //setup roles
            IsServer = _channel.ChannelRole == YPChannel.ChannelRole.Server;
            IsClient = _channel.ChannelRole == YPChannel.ChannelRole.Client;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        private bool YPChannel_Negotiation(YPChannel.Channel Channel)
        {
            DebugEx.TraceLog($"RemoteNode : Negotiation started with node. (ip:{IPAddress} port:{RemotePort} nodekey:{RemoteNodeKey})");

            //check
            if (!IsServer)
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

            //Add node to discovered
            lock (NodeDiscoverManager.RemoteNodes)
            {
                var remInfo = NodeDiscoverManager.RemoteNodes.TryGetOrDefault(RemoteEndpointID);
                if (remInfo != null)
                    return false; //close channel since we already have one
                else
                {
                    //add me to active remote nodes
                    NodeDiscoverManager.RemoteNodes.Add(RemoteEndpointID, this);
                    //raise event
                    NodeDiscoverManager._raiseAssiciationEvent(this);
                }
            }

            //all ok
            return true;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        private void YPChannel_OnOpenEvent(YPChannel.Channel Channel)
        {
            State = RemoteNodeState.Connected;
            DebugEx.TraceLog($"RemoteNode : Openned channel with node. (ip:{IPAddress} port:{RemotePort} nodekey:{RemoteNodeKey})");
        }
        //-------------------------------------------------------------------------------------------------------------------------
        private void YPChannel_OnClosedEvent(YPChannel.Channel Channel)
        {
            State = RemoteNodeState.Disconnected;
            _disconnect();
        }
        //-------------------------------------------------------------------------------------------------------------------------
        private void YPChannel_OnMessageReceived(YPChannel.Channel Channel, YPChannel.YPMessage Message)
        {
            var msg = Message.Payload;
            //handle negotiation
            if (msg is AssociationRequest && IsClient)
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
            else if (msg is AuthenticationRequest && IsClient)
            {
                //...
            }
            else
                DebugEx.Assert("Unkown message received");
        }
        //-------------------------------------------------------------------------------------------------------------------------
        internal void _disconnect()
        {
            try
            {
                State = RemoteNodeState.Disconnected;

                //close client and channel (if server)
                try { Channel?.Close(); } catch { }
                try { _YPClient?.Close(); } catch { }

                //let go of references
                Channel = null;
                _YPClient = null;
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "Unhandled exception in disconnection"); }
        }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
