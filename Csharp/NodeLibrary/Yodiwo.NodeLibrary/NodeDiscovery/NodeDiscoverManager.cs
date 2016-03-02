using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.YPChannel.Transport.Sockets;

#if UNIVERSAL
using Windows.ApplicationModel;
#endif

namespace Yodiwo.NodeLibrary.NodeDiscovery
{
    public class NodeDiscoverManager
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public Node Node;
        //------------------------------------------------------------------------------------------------------------------------
        public const int BroadcastPort = 23630;
        //------------------------------------------------------------------------------------------------------------------------
        public readonly LANDiscoverer Discoverer;
        //------------------------------------------------------------------------------------------------------------------------
        public const int DefaultYPCPort = 16086;
        public readonly int YPCPort;
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsRunning => Discoverer.IsRunning;
        //------------------------------------------------------------------------------------------------------------------------
        public DiscoveryMessage DiscoveryMessage = new DiscoveryMessage();
        //------------------------------------------------------------------------------------------------------------------------
        DictionaryTS<LANDiscoverer.RemoteEndpointID, RemoteNode> connectingNodes = new DictionaryTS<LANDiscoverer.RemoteEndpointID, RemoteNode>();
        //------------------------------------------------------------------------------------------------------------------------
        public readonly DictionaryTS<LANDiscoverer.RemoteEndpointID, RemoteNode> RemoteNodes = new DictionaryTS<LANDiscoverer.RemoteEndpointID, RemoteNode>();
        //------------------------------------------------------------------------------------------------------------------------
        YPChannel.Transport.Sockets.Server _YPServer;
        public YPChannel.Transport.Sockets.Server YPServer => _YPServer;
        //------------------------------------------------------------------------------------------------------------------------
        public static YPChannel.Protocol Protocol = new YPChannel.Protocol()
        {
            Version = 1,
            ProtocolDefinitions = new List<YPChannel.Protocol.MessageTypeGroup>()
            {
                //Basic group
                new YPChannel.Protocol.MessageTypeGroup()
                {
                    GroupName = "NodeLibrary.BrotherhoodAPI",
                    MessageTypes = new[]
                    {
                        typeof(AssociationRequest),
                        typeof(AssociationResponse),
                    },
                }
            },
        };
        //------------------------------------------------------------------------------------------------------------------------
        bool _IsInitialized = false;
        public bool IsInitialized => _IsInitialized;
        //------------------------------------------------------------------------------------------------------------------------
        public delegate void OnRemoteNodeDiscoveryDelegate(RemoteNode endpoint);
        public event OnRemoteNodeDiscoveryDelegate OnRemoteNodeDiscovery;

        public delegate void OnRemoteNodeAssociationDelegate(RemoteNode endpoint);
        public event OnRemoteNodeAssociationDelegate OnRemoteNodeAssociation;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public NodeDiscoverManager(Node Node, int YPCPort = DefaultYPCPort)
        {
            this.Node = Node;
            this.YPCPort = YPCPort;

            //build flags
            var flags = NodeFlags.None;
            if (Node.CanSolveGraphs) flags |= NodeFlags.CanSolveGraphs;

            //create outgoing message
#if NETFX
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            DiscoveryMessage = new DiscoveryMessage()
            {
                ProtocolVersion = Protocol.Version,
                MajorVersion = version.Major,
                MinorVersion = version.Minor,
                BuildVersion = version.Build,
                Flags = (int)flags,
            };
#else
            DiscoveryMessage = new DiscoveryMessage()
            {
                ProtocolVersion = Protocol.Version,
                MajorVersion = Package.Current.Id.Version.Major,
                MinorVersion = Package.Current.Id.Version.Minor,
                BuildVersion = Package.Current.Id.Version.Build,
                Flags = (int)flags,
            };
#endif

            //create discoverer
            Discoverer = new YPChannel.Transport.Sockets.LANDiscoverer(DiscoveryMessage, BroadcastPort: BroadcastPort);
            Discoverer.OnEndpointMsgRx += Discoverer_OnEndpointMsgRx;
            Discoverer.OnEndpointTimeout += Discoverer_OnEndpointTimeout;

            //hook event for shutdown
            YPChannel.Channel.OnSystemShutDownRequest.Add(Yodiwo.WeakAction<object>.Create(Deinitialize));
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Initialize()
        {
            lock (this)
            {
                //deinit if already initialized
                if (_IsInitialized)
                    Deinitialize();

                try
                {
                    //update message
                    Node.NodeKey.GetBytes(DiscoveryMessage.NodeKey, 0);
                    DiscoveryMessage.YPChannelPort = YPCPort;
                    Discoverer.UpdateDiscoveryMessage(DiscoveryMessage);

                    //init discoverer
                    Discoverer.Initialize(true, YPCPort != 0);

                    //start ypserver
                    if (YPCPort != 0)
                    {
                        _YPServer = new YPChannel.Transport.Sockets.Server(Protocol);
                        _YPServer.OnNewChannel += _YPServer_OnNewChannel;
                        _YPServer.Start(YPCPort);
                    }

                    //mark
                    _IsInitialized = true;
                }
                catch (Exception ex)
                {
                    Deinitialize();
                    DebugEx.Assert(ex);
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Deinitialize()
        {
            lock (this)
            {
                try
                {
                    //close socket
                    try { Discoverer.Deinitialize(); } catch { }

                    //close server
                    _YPServer?.Stop(CloseAllChannels: true);
                }
                catch (Exception ex) { DebugEx.Assert(ex); }
                finally { _IsInitialized = false; }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void Discoverer_OnEndpointMsgRx(YPChannel.Transport.Sockets.LANDiscoverer.RemoteEndpointInfo endpoint, YPChannel.Transport.Sockets.DiscoveryMessageBase __msg)
        {
            try
            {
                //get valid msg
                var msg = __msg as DiscoveryMessage;
                if (msg == null)
                    return;

                //examine existing association
                lock (RemoteNodes)
                {
                    var remInfo = RemoteNodes.TryGetOrDefault(endpoint.ID);
                    if (remInfo == null)
                    {
                        //try to find if is connecting
                        remInfo = connectingNodes.TryGetOrDefault(endpoint.ID);
                        if (remInfo == null)
                        {
                            //create entry for remote node
                            remInfo = new RemoteNode(Node, this)
                            {
                                RemoteEndpointID = endpoint.ID,
                                DiscoveryMessage = msg,
                                State = RemoteNode.RemoteNodeState.Unkown,
                                RemoteNodeKey = API.Plegma.NodeKey.FromBytes(msg.NodeKey, 0),
                            };
                            if (remInfo.RemoteNodeKey.IsInvalid)
                                return;

                            //higher id is server
                            if (DiscoveryMessage.Id > endpoint.ID.ID && YPCPort != 0)
                                return; //let the other node connect on me

                            //add to connecting nodes 
                            connectingNodes.Add(endpoint.ID, remInfo);

                            //raise event
                            if (OnRemoteNodeDiscovery != null)
                                Task.Run(() => OnRemoteNodeDiscovery?.Invoke(remInfo));

                            //Start a connection attempt
                            Task.Run(() => _StartConnectionAttempt(remInfo, msg));
                        }
                    }
                    else
                    {
                        //start remote node connection
                        if (remInfo.State == RemoteNode.RemoteNodeState.Disconnected)
                            Task.Run(() => { try { remInfo.Connect(); } catch (Exception exx) { DebugEx.Assert(exx); } });
                    }
                }
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "DiscoveryTaskEntryPoint error"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        void _StartConnectionAttempt(RemoteNode remInfo, DiscoveryMessage msg)
        {
            try
            {
                //inform
                DebugEx.TraceLog($"NodeDiscoverer : Discovered new node. (ip:{remInfo.IPAddress} nodekey:{msg.NodeKey})");

                //start remote node connection
                if (remInfo.Connect(ignoreReconnectionPeriod: true))
                {
                    //Success !
                    lock (RemoteNodes)
                    {
                        if (RemoteNodes.ContainsKey(remInfo.RemoteEndpointID) == false)
                        {
                            //add to lookup as a valid remote node
                            RemoteNodes.Add(remInfo.RemoteEndpointID, remInfo);
                            //raise event
                            if (OnRemoteNodeAssociation != null)
                                Task.Run(() => OnRemoteNodeAssociation?.Invoke(remInfo));
                        }
                        else
                            remInfo._disconnect();
                    }
                }
            }
            catch (Exception exx) { DebugEx.Assert(exx); }
            finally
            {
                //connection attempt finish (success or fail does not matter)
                connectingNodes.Remove(remInfo.RemoteEndpointID);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void Discoverer_OnEndpointTimeout(LANDiscoverer.RemoteEndpointInfo endpoint)
        {
            try
            {
                var remInfo = RemoteNodes.TryGetOrDefault(endpoint.ID);
                if (remInfo != null)
                {
                    remInfo._disconnect();
                    RemoteNodes.Remove(endpoint.ID);
                }
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "Discoverer_OnEndpointTimeout error"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void _YPServer_OnNewChannel(Server Server, YPChannel.Channel Channel)
        {
            var id = new LANDiscoverer.RemoteEndpointID()
            {
                IPAddress = Channel.RemoteIdentifier,
                ID = 0,
            };
            //create entry for remote node
            var remInfo = new RemoteNode(Node, this)
            {
                RemoteEndpointID = id,
                DiscoveryMessage = null,
                State = RemoteNode.RemoteNodeState.Unkown,
                RemoteNodeKey = default(NodeKey),
            };
            //setup channel
            remInfo.SetupChannel(Channel);
        }
        //------------------------------------------------------------------------------------------------------------------------
        internal void _raiseAssiciationEvent(RemoteNode remInfo)
        {
            //raise event
            if (OnRemoteNodeAssociation != null)
                Task.Run(() => OnRemoteNodeAssociation?.Invoke(remInfo));
        }
        //------------------------------------------------------------------------------------------------------------------------
        ~NodeDiscoverManager()
        {
            try
            {
                if (IsRunning)
                    Deinitialize();
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "~NodeDiscovery error"); }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
