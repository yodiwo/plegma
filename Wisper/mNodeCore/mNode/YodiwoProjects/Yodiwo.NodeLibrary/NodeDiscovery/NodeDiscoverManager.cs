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
        public readonly DictionaryTS<LANDiscoverer.RemoteEndpointID, RemoteNode> RemoteNodes = new DictionaryTS<LANDiscoverer.RemoteEndpointID, RemoteNode>();
        public readonly DictionaryTS<NodeKey, RemoteNode> BrotherNodes = new DictionaryTS<NodeKey, RemoteNode>();
        //------------------------------------------------------------------------------------------------------------------------
        YPChannel.Transport.Sockets.Server _YPServer;
        public YPChannel.Transport.Sockets.Server YPServer => _YPServer;
        //------------------------------------------------------------------------------------------------------------------------
        public static YPChannel.Protocol Protocol = new YPChannel.Protocol()
        {
            Version = 1,
            ProtocolDefinitions = new List<YPChannel.Protocol.MessageTypeGroup>()
            {
                //Plegma API group
                new YPChannel.Protocol.MessageTypeGroup() { GroupName = API.Plegma.PlegmaAPI.ApiGroupName,      MessageTypes = API.Plegma.PlegmaAPI.ApiMessages      },
                new YPChannel.Protocol.MessageTypeGroup() { GroupName = API.Plegma.PlegmaAPI.ApiLogicGroupName, MessageTypes = API.Plegma.PlegmaAPI.LogicApiMessages },
                new YPChannel.Protocol.MessageTypeGroup() { GroupName = API.MediaStreaming.Video.ApiGroupName,  MessageTypes = API.MediaStreaming.Video.ApiMessages  },
                new YPChannel.Protocol.MessageTypeGroup() { GroupName = API.MediaStreaming.Audio.ApiGroupName,  MessageTypes = API.MediaStreaming.Audio.ApiMessages  },

                //BrotherHood api group
                new YPChannel.Protocol.MessageTypeGroup()
                {
                    GroupName = "NodeLibrary.BrotherhoodAPI",
                    MessageTypes = new[]
                    {
                        typeof(AssociationRequest),
                        typeof(AssociationResponse),
                    },
                },
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

        public delegate void OnVBMReceivedDelegate(NodeKey BrotherNode, VirtualBlockEventMsg msg);
        public event OnVBMReceivedDelegate OnVBMReceived = null;
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
                        var mode = YPChannel.ChannelSerializationMode.Json;
                        _YPServer = new YPChannel.Transport.Sockets.Server(Protocol, SupportedChannelSerializationModes: mode, PreferredChannelSerializationModes: mode);
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
        private void Discoverer_OnEndpointMsgRx(YPChannel.Transport.Sockets.LANDiscoverer.RemoteEndpointInfo endpoint, YPChannel.Transport.Sockets.IDiscoveryMessageBase __msg)
        {
            try
            {
                //get valid msg
                var msg = __msg as DiscoveryMessage;
                if (msg == null)
                    return;

                //create remotenodekey
                var rem_nodekey = API.Plegma.NodeKey.FromBytes(msg.NodeKey, 0);
                if (rem_nodekey.IsInvalid)
                    return;

                //examine existing association
                lock (RemoteNodes)
                {
                    var remInfo = RemoteNodes.TryGetOrDefault(endpoint.ID);
                    if (remInfo == null)
                    {
                        //inform
                        DebugEx.TraceLog($"NodeDiscoverer : Discovered new node. (ip:{endpoint.IPAddress} nodekey:{rem_nodekey})");

                        //create entry for remote node
                        remInfo = new RemoteNode(Node, this)
                        {
                            RemoteEndpointID = endpoint.ID,
                            DiscoveryMessage = msg,
                            RemoteNodeKey = rem_nodekey,
                        };
                        //add to discovered remote nodes 
                        RemoteNodes.Add(endpoint.ID, remInfo);

                        //hookevents
                        hookNewRemoteNodeEvents(remInfo);

                        //Start a connection attempt
                        remInfo.StartConnectionTask();

                        //raise event
                        if (OnRemoteNodeDiscovery != null)
                            Task.Run(() => { try { OnRemoteNodeDiscovery?.Invoke(remInfo); } catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception"); } });
                    }
                    else
                    {
                        //start remote node connection
                        if (!remInfo.IsDisposed &&
                            (remInfo.ClientChannel == null || (remInfo.ClientChannel.State == YPChannel.ChannelStates.Closed && !remInfo.ConnectionTaskRunning)))
                            remInfo.StartConnectionTask();
                    }
                }
            }
            catch (Exception ex) { DebugEx.TraceError(ex, "DiscoveryTaskEntryPoint error"); }
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

            lock (RemoteNodes)
            {
                //get or create entry for remote node
                var remInfo = RemoteNodes.TryGetOrDefault(id);
                if (remInfo == null)
                {
                    //create entry for remote node
                    remInfo = new RemoteNode(Node, this)
                    {
                        RemoteEndpointID = id,
                        DiscoveryMessage = null,
                        RemoteNodeKey = default(NodeKey),
                    };

                    //add to discovered remote nodes 
                    RemoteNodes.ForceAdd(id, remInfo);
                }

                //hookevents
                hookNewRemoteNodeEvents(remInfo);

                //setup channel
                remInfo.SetupChannel(Channel);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        void hookNewRemoteNodeEvents(RemoteNode remInfo)
        {
            remInfo.OnChannelOpen += RemoteNode_OnChannelOpen;
            remInfo.OnChannelClose += RemoteNode_OnChannelClose;
            remInfo.OnVBMReceived += RemoteNode_OnVBMReceived;
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void RemoteNode_OnChannelOpen(RemoteNode RemoteNode)
        {
            BrotherNodes.Add(RemoteNode.RemoteNodeKey, RemoteNode);
            //raise event
            if (OnRemoteNodeAssociation != null)
                Task.Run(() => { try { OnRemoteNodeAssociation?.Invoke(RemoteNode); } catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception"); } });
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void RemoteNode_OnChannelClose(RemoteNode RemoteNode)
        {
            if (!RemoteNode.IsConnected)
                BrotherNodes.Remove(RemoteNode.RemoteNodeKey);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void RemoteNode_OnVBMReceived(RemoteNode RemoteNode, VirtualBlockEventMsg msg)
        {
            OnVBMReceived?.Invoke(RemoteNode.RemoteNodeKey, msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void SendVBMToBrothers(List<VirtualBlockEvent> events)
        {
            try
            {
                //split into packets-per brother
                var brotherPackets = new Dictionary<NodeKey, List<VirtualBlockEvent>>();
                foreach (var ev in events)
                {
                    var bk = (BlockKey)ev.BlockKey;
                    var nk = bk.GraphKey.NodeKey;
                    if (BrotherNodes.ContainsKey(nk))
                    {
                        //get (or create) packet list
                        var packets = brotherPackets.TryGetOrDefault(nk);
                        if (packets == null)
                            brotherPackets.Add(nk, packets = new List<VirtualBlockEvent>());
                        //add packet for brother node
                        packets.Add(ev);
                    }
                }

                //send to brothers and consume
                foreach (var bpkv in brotherPackets)
                {
                    //get brother
                    var brother = BrotherNodes.TryGetOrDefault(bpkv.Key);
                    if (brother != null && brother.IsConnected)
                    {
                        var msgReq = new VirtualBlockEventMsg()
                        {
                            BlockEvents = bpkv.Value.ToArray(),
                        };
                        var rsp = brother.SendRequest<GenericRsp>(msgReq, Timeout: TimeSpan.FromSeconds(3));
                        if (rsp != null && rsp.IsSuccess)
                        {
                            //consume events from original set
                            msgReq.BlockEvents.ForEach(e => events.Remove(e));
                        }
                    }
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "Unhandled exception in SendVBMToBrothers"); }
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
