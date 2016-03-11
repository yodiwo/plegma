using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Yodiwo.API.Plegma;
using Yodiwo.Logic;
using Yodiwo.Logic.Blocks.Things;
using Yodiwo.Logic.Descriptors;
using Yodiwo.NodeLibrary;
using Newtonsoft.Json;

namespace Yodiwo.NodeLibrary.Graphs
{
    public class NodeGraphManager : Yodiwo.NodeLibrary.Graphs.INodeGraphManager
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public const string DataIdentifier_Graphs = "graphs.json";
        //------------------------------------------------------------------------------------------------------------------------
        public object locker = new object();
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsInitialized = false;
        //------------------------------------------------------------------------------------------------------------------------
        Type[] BlockLibrarians = null;
        //------------------------------------------------------------------------------------------------------------------------
        private Node _Node;
        public Node Node { get { return _Node; } }
        //------------------------------------------------------------------------------------------------------------------------
        GraphManager _GraphManager;
        public GraphManager GraphManager { get { return _GraphManager; } }
        //------------------------------------------------------------------------------------------------------------------------
        [Serializable]
        public class GraphInfo
        {
            public GraphKey GraphKey;
            public string GraphDescriptorString;
            [JsonIgnore]
            [NonSerialized]
            public GraphDescriptor GraphDescriptor;
            [JsonIgnore]
            [NonSerialized]
            public Graph Graph;
        }
        DictionaryTS<GraphKey, GraphInfo> Graphs = new DictionaryTS<GraphKey, GraphInfo>();
        //------------------------------------------------------------------------------------------------------------------------
        DictionaryTS<ThingKey, HashSetTS<BlockKey>> ThingKey2BlockKey = new DictionaryTS<ThingKey, HashSetTS<BlockKey>>();
        DictionaryTS<ThingKey, HashSetTS<BlockKey>> ThingKey2ThingInBlockKey = new DictionaryTS<ThingKey, HashSetTS<BlockKey>>();
        //------------------------------------------------------------------------------------------------------------------------
#if NETFX
        public string[] BlockLibrariesNames { get { return BlockLibrarians == null || BlockLibrarians.Length == 0 ? null : BlockLibrarians.Select(t => t.Assembly.NamePortable()).ToArray(); } }
#else
        public string[] BlockLibrariesNames { get { return BlockLibrarians == null || BlockLibrarians.Length == 0 ? null : BlockLibrarians.Select(t => t.GetTypeInfo().Assembly.NamePortable()).ToArray(); } }
#endif
        //------------------------------------------------------------------------------------------------------------------------
        HashSetTS<PortKey> _ActivePortKeys = new HashSetTS<PortKey>();
        public IReadOnlySet<PortKey> ActivePortKeys => _ActivePortKeys;

        HashSetTS<ThingKey> _ActiveThingKeys = new HashSetTS<ThingKey>();
        public IReadOnlySet<ThingKey> ActiveThingKeys => _ActiveThingKeys;

        HashSetTS<Port> _ActivePorts = new HashSetTS<Port>();
        public IReadOnlySet<Port> ActivePorts => _ActivePorts;

        HashSetTS<Thing> _ActiveThings = new HashSetTS<Thing>();
        public IReadOnlySet<Thing> ActiveThings => _ActiveThings;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Constructors
        //------------------------------------------------------------------------------------------------------------------------
        public NodeGraphManager(IEnumerable<Type> BlockLibrarians)
        {
            //keep
            this.BlockLibrarians = BlockLibrarians.ToArray();

            //Create Graph Manager
            _GraphManager = new GraphManager(Residency.Node) { Name = "NodeLibrary Graph Manager" };
            //Setup GraphManager
            GraphManager.Loader = OnGraphLoadReq;
            GraphManager.Saver = GraphSaveReq;

            //initialize librarians
            if (this.BlockLibrarians != null && this.BlockLibrarians.Length > 0)
            {
                //add librarians to graph manager
                GraphManager.AddLibrarians(this.BlockLibrarians);

                //setup basic librarian
                GraphManager.GetLibrarian<Logic.BlockLibrary.Basic.Librarian>().VirtualOutputMsgHandler = VirtualOutputMsgHandler;
                GraphManager.GetLibrarian<Logic.BlockLibrary.Basic.Librarian>().VirtualOutputBatchMsgHandler = VirtualOutputBatchMsgHandler;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion


        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        void VirtualOutputMsgHandler(Logic.Blocks.Endpoints.Out.VirtualOutput.VirtualIOMsg ev)
        {
            //TODO:..
            DebugEx.Assert("TODO");
        }
        //------------------------------------------------------------------------------------------------------------------------
        void VirtualOutputBatchMsgHandler(Logic.Blocks.Endpoints.Out.VirtualOutput.VirtualIOMsg[] ev)
        {
            try
            {
                //send virtual block event msg
                var msg = new VirtualBlockEventMsg()
                {
                    BlockEvents = ev.Where(e => e.RemoteVirtualInputBlockKey.IsValid)
                                    .Select(e => new VirtualBlockEvent()
                                    {
                                        BlockKey = e.RemoteVirtualInputBlockKey,
                                        Indices = e.Indices,
                                        Values = e.Values.Select(v => v.ToJSON()).ToArray(),
                                        RevNum = e.Revision,
                                    }).ToArray(),
                };
                if (msg.BlockEvents.Length == 0)
                    return;

                //send message
                Node.SendMessage(msg);

            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Initialize(Node ParentNode)
        {
            lock (locker)
            {
                //check
                if (IsInitialized)
                    return;

                //keep
                this._Node = ParentNode;

                //start
                GraphManager.Start();

                //register for thing solve
                YEventRouter.EventRouter.AddEventHandler<Logic.Blocks.Things.EvThingSolved>(OnThingSolvedCb, 100);

                //set flag
                IsInitialized = true;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void OnConnectedToCloud()
        {
            try
            {
                //inform cloud about localy deployed graphs
                var msg = new LocallyDeployedGraphsMsg()
                {
                    DeployedGraphKeys = Graphs.Keys.Select(k => k.ToStringInvariant()).ToArray(),
                };
                Node.SendMessage(msg);
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void DeployGraphs()
        {
            //Load+deploy all graphs
            try
            {
                var graphInfos = Node.LoadObject<Dictionary<string, string>>(DataIdentifier_Graphs, Secure: true);
                //deploy them
                if (graphInfos != null)
                    foreach (var entry in graphInfos)
                    {
                        var deploymsg = new GraphDeploymentReq()
                        {
                            GraphKey = entry.Key,
                            GraphDescriptor = entry.Value,
                            IsDeployed = true,
                        };
                        HandleGraphDeploymentReq(deploymsg, true);
                    }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Save()
        {
            try
            {
                lock (locker)
                {
                    //compile data
                    var data = new Dictionary<string, string>();
                    foreach (var entry in Graphs)
                        data.ForceAdd(entry.Key, entry.Value.GraphDescriptorString);
                    //save
                    if (Node.SaveObject(DataIdentifier_Graphs, data, Secure: true) == false)
                        DebugEx.TraceError("Could not save graphs");
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Unhandled exception caught");
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public Graph BuildGraph(GraphDescriptor graphDescriptor)
        {
            lock (locker)
            {
                //check
                if (graphDescriptor == null)
                {
                    DebugEx.Assert("Null detected");
                    return null;
                }

                //Build Graph
                var builderResult = GraphBuilder.BuildFromDescriptor(GraphManager, graphDescriptor,
                    (bmvType, blockMV) =>
                    {
                        // Handle YThing-constructed instantiation
                        if (typeof(Logic.ThingBase).IsAssignableFrom(bmvType))
                        {
                            //find thing key
                            var thingKey = blockMV.Properties.Fields.Find(x => x.Name == nameof(Logic.ThingBase.ThingKey)).Value as string;

                            var thing = Node.Things.TryGetOrDefaultReadOnly(thingKey);
                            if (thing == null)
                            {
                                DebugEx.Assert("GenerateGraphFromModelView:: Thing with requested ThingKey" + thingKey + " not found");
                                return null;
                            }

                            return bmvType
                                .GetConstructor(new[] { typeof(Thing) })
                                .Invoke(new[] { thing }) as Block;
                        }
                        else
                            return null;
                    });

                //Check results
                DebugEx.Assert(builderResult.Graph != null, "Could not build graph");
                return builderResult.Graph;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool OnGraphLoadReq(GraphManager.GraphLoadRequestData req, out Graph Graph)
        {
            lock (locker)
            {
                Graph = null;
                var gi = Graphs.TryGetOrDefault(req.GraphKey);
                if (gi == null)
                    return false;
                else
                {
                    Graph = gi.Graph;
                    return Graph != null;
                }
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        void GraphSaveReq(GraphManager.GraphSaveRequestData request)
        {
            lock (locker)
            {
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void HandlePortStates(IEnumerable<TupleS<Port, string>> States)
        {
            lock (locker)
            {
                List<Yodiwo.Logic.GraphManager.SimultaneousActionRequest> simReq = null;

                foreach (var portEvent in States)
                {
                    var port = portEvent.Item1;
                    var portKey = (PortKey)port.PortKey;
                    var state = portEvent.Item2;
                    var thingKey = portKey.ThingKey;

                    //checks
                    if (port == null)
                    {
                        DebugEx.TraceError("Invalid portkey");
                        continue;
                    }
                    if (portKey.IsInvalid)
                    {
                        DebugEx.TraceError("Invalid portkey");
                        continue;
                    }

                    //find thing block set
                    var blockSet = ThingKey2ThingInBlockKey.TryGetOrDefault(thingKey);
                    if (blockSet == null)
                        continue;

                    //Update state and revision number
                    lock (port)
                    {
                        //Interlocked.Increment(ref port.RevNum); //TODO: should increase?
                        port.State = state;
                    }

                    //create action update message
                    var data = new Logic.Blocks.Things.ThingUpdateAction
                    {
                        PortKey = portKey,
                        PortState = state,
                    };

                    //create a simultaneous action requests
                    foreach (var bk in blockSet)
                    {
                        var _req = new Yodiwo.Logic.GraphManager.SimultaneousActionRequest()
                        {
                            BlockKey = bk,
                            BlockActionData = data,
                        };
                        //add to simReq
                        if (simReq == null)
                            simReq = new List<GraphManager.SimultaneousActionRequest>();
                        simReq.Add(_req);
                        DebugEx.TraceLog("send event for thing " + thingKey + " block " + bk);
                    }
                }

                //send action request
                if (simReq != null && simReq.Count > 0)
                    GraphManager.RequestGraphAction(simReq);

                //done
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public GenericRsp HandleGraphDeploymentReq(GraphDeploymentReq req)
        {
            return HandleGraphDeploymentReq(req, false);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public GenericRsp HandleGraphDeploymentReq(GraphDeploymentReq req, bool SupressSave)
        {
            lock (locker)
            {
                var res = new GenericRsp();
                try
                {
                    //get graphkey
                    var graphkey = (GraphKey)req.GraphKey;
                    if (graphkey.IsInvalid)
                    {
                        res.IsSuccess = false;
                        res.Message = "Invalid GraphKey";
                        return res;
                    }
                    if (graphkey.NodeId != this.Node.NodeKey.NodeID)
                    {
                        res.IsSuccess = false;
                        res.Message = "Invalid NodeID in GraphKey";
                        return res;
                    }

                    //collect sets
                    var sets = Node.BeginActiveThingsUpdate();

                    //deploy or undeploy?
                    if (req.IsDeployed)
                    {
                        //deserialize graph descriptor
                        GraphDescriptor graphDescriptor;
                        try { graphDescriptor = req.GraphDescriptor.FromJSON<GraphDescriptor>(); }
                        catch (Exception ex)
                        {
                            DebugEx.Assert(ex, "Could not deserialize graph descriptor");
                            res.IsSuccess = false;
                            res.Message = "Could not deserialize graph descriptor";
                            return res;
                        }
                        graphDescriptor.GraphKey = graphkey;

                        //Build Graph
                        Graph graph;
                        try { graph = BuildGraph(graphDescriptor); }
                        catch (Exception ex)
                        {
                            DebugEx.Assert(ex, "Could not build graph from graph descriptor (unhandled exception)");
                            res.IsSuccess = false;
                            res.Message = "Could not  build graph from graph descriptor (unhandled exception)";
                            return res;
                        }
                        if (graph == null)
                        {
                            DebugEx.Assert("Could not build graph from graph descriptor");
                            res.IsSuccess = false;
                            res.Message = "Could not  build graph from graph descriptor";
                            return res;
                        }

                        //set key
                        graph.GraphKey = graphkey;

                        //do i already have it?
                        if (Graphs.ContainsKey(graphkey))
                        {
                            //invalidate graph
                            _GraphManager.InvalidateGraph(graphkey);
                            //remove information
                            Graphs.Remove(graphkey);
                        }

                        //try deploy graph
                        try
                        {
                            graph.OnDeploy(true, null);
                        }
                        catch (Exception ex)
                        {
                            DebugEx.Assert(ex, "Graph OnDeploy() failed");
                            res.IsSuccess = false;
                            res.Message = "Graph OnDeploy() failed. Message : " + ex.Message;
                            return res;
                        }

                        //add to lookup
                        var gi = new GraphInfo()
                        {
                            GraphKey = graphkey,
                            GraphDescriptor = graphDescriptor,
                            GraphDescriptorString = req.GraphDescriptor,
                            Graph = graph,
                        };
                        Graphs.Add(graphkey, gi);

                        //save!
                        if (IsInitialized && !SupressSave)
                            Save();

                        //associate block keys
                        foreach (var thingblock in graph.Blocks.OfType<ThingBase>())
                        {
                            //Add to Thing2Block set
                            {
                                //find block key set
                                var set = ThingKey2BlockKey.TryGetOrDefault(thingblock.ThingKey);
                                if (set == null)
                                {
                                    set = new HashSetTS<BlockKey>();
                                    ThingKey2BlockKey.Add(thingblock.ThingKey, set);
                                }
                                //add to loockup set
                                set.Add(thingblock.BlockKey);
                            }

                            //Add only for thingIn
                            if (thingblock.IsThingIn)
                            {
                                //find block key set
                                var set = ThingKey2ThingInBlockKey.TryGetOrDefault(thingblock.ThingKey);
                                if (set == null)
                                {
                                    set = new HashSetTS<BlockKey>();
                                    ThingKey2ThingInBlockKey.Add(thingblock.ThingKey, set);
                                }
                                //add to loockup set
                                set.Add(thingblock.BlockKey);
                            }
                        }

                        //Handle Deploy
                        res.IsSuccess = true;
                        res.Message = "Graph Deployed Successfully";
                    }
                    else
                    {
                        //Handle UnDeploy
                        if (Graphs.ContainsKey(graphkey) == false)
                        {
                            res.IsSuccess = true;
                            res.Message = "Graph Undeployed Successfully (was not deployed)";
                        }
                        else
                        {
                            //get graph
                            var gi = Graphs[graphkey];
                            var graph = gi.Graph;
                            //inform graph
                            try { graph.OnUndeploy(null); } catch (Exception ex) { DebugEx.Assert(ex, "Graph OnUndeploy failed"); }
                            //invalidate graph
                            _GraphManager.InvalidateGraph(graphkey);
                            //remove information
                            Graphs.Remove(graphkey);
                            //save!
                            if (IsInitialized && !SupressSave)
                                Save();
                            //disassociate block keys
                            if (graph != null)
                                foreach (var thingblock in gi.Graph.Blocks.OfType<ThingBase>())
                                {
                                    //remove from thing2block
                                    {
                                        var set = ThingKey2BlockKey.TryGetOrDefault(thingblock.ThingKey);
                                        if (set != null)
                                            set.Remove(thingblock.BlockKey);
                                    }

                                    //remove from thignIn2block
                                    if (thingblock.IsThingIn)
                                    {
                                        var set = ThingKey2ThingInBlockKey.TryGetOrDefault(thingblock.ThingKey);
                                        if (set != null)
                                            set.Remove(thingblock.BlockKey);
                                    }
                                }
                            //done
                            res.IsSuccess = true;
                            res.Message = "Graph Undeployed Successfully";
                        }
                    }

                    //finish update
                    Node.EndActiveThingsUpdate(sets);
                }
                catch (Exception ex)
                {
                    res.IsSuccess = false;
                    res.Message = "Unhandled exception in GraphDeploymentReq(). Message=" + ex.Message;
                }
                finally
                {
                    //update active ports/things
                    var activeThings = ThingKey2BlockKey.Where(kv => kv.Value.Count > 0)
                                                        .Select(kv => Node.Things.TryGetOrDefaultReadOnly(kv.Key))
                                                        .WhereNotNull().ToHashSetTS();
                    var activeThingKeys = activeThings.Select(t => (ThingKey)t.ThingKey).ToHashSetTS();

                    var activePorts = activeThings.SelectMany(t => t.Ports).ToHashSetTS();
                    var activePortsKeys = activePorts.Select(p => (PortKey)p.PortKey).ToHashSetTS();

                    //update sets
                    Interlocked.Exchange(ref _ActiveThings, activeThings);
                    Interlocked.Exchange(ref _ActivePorts, activePorts);
                    Interlocked.Exchange(ref _ActiveThingKeys, activeThingKeys);
                    Interlocked.Exchange(ref _ActivePortKeys, activePortsKeys);
                }
                //return result msg
                return res;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Called by Logic's Things Manager Subsystem when an EndpointOut is solved
        /// </summary>
        public void OnThingSolvedCb(object sender, YEventRouter.EvInfo evInfo, Logic.Blocks.Things.EvThingSolved ev)
        {
            //filter out invalid residency
            if (ev.Residency != Residency.Node)
                return;

            //check
            if (ev.PortsUpdated == null || !ev.PortsUpdated.Any())
            {
                DebugEx.Assert("Invalid input");
                return;
            }

            //send event to node
            var msg = new PortEventMsg()
            {
                PortEvents = ev.PortsUpdated.Select(p => new API.Plegma.PortEvent(p.PortKey, p.PortState, revNum: p.RevNum)).ToArray()
            };
            Node.HandlePortEventMsg(msg);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public bool IsPortActive(PortKey pk) { return _ActivePortKeys?.Contains(pk) ?? false; }
        public bool IsThingActive(ThingKey tk) { return _ActiveThingKeys?.Contains(tk) ?? false; }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
