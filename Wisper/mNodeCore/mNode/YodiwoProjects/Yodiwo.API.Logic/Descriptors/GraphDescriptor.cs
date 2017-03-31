using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Newtonsoft.Json;

namespace Yodiwo.Logic.Descriptors
{
    public class GraphDescriptor
    {
        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        public Yodiwo.API.Plegma.GraphDescriptorKey GraphDescriptorKey;

        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public List<BlockDescriptor> Blocks;

        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public List<ConnectionDescriptor> Connections;

        [JsonIgnore]
        public List<ConnectionDescriptor> RealConnections
        {
            get
            {
                try
                {
                    // check if the graph contains any linkPoints
                    if (LinkPoints != null && LinkPoints.Count > 0)
                    {
                        // get all blocks' uids
                        List<int> blockUIDs = Blocks?.Select(x => x.UID).ToList();

                        // get all connections from Block to Block
                        var block2BlockConnections = Connections.FindAll(x => blockUIDs.Contains(x.FromID) && blockUIDs.Contains(x.ToID));

                        // create 'RealConnections' list 
                        var realConnections = new List<ConnectionDescriptor>();

                        // add connections between blocks
                        realConnections.AddFromSource(block2BlockConnections);

                        // validate linkPoints
                        List<ConnectionDescriptor> connections2LinkPoints, connectionsFromLinkPoints;
                        if (_LinkPointsHelpers(out connections2LinkPoints, out connectionsFromLinkPoints))
                        {
                            foreach (var connection in connections2LinkPoints)
                            {
                                // get input point
                                var inputPoint = LinkPoints.Find(x => x.UID == connection.ToID);
                                if (inputPoint != null)
                                {
                                    // get output point
                                    var outputPoint = connectionsFromLinkPoints.Find(x => x.FromID == inputPoint.ConnectedPointUID);
                                    if (outputPoint != null)
                                    {
                                        realConnections.Add(new ConnectionDescriptor()
                                        {
                                            FromID = connection.FromID,
                                            FromIndex = connection.FromIndex,
                                            ToID = outputPoint.ToID,
                                            ToIndex = outputPoint.ToIndex
                                        });
                                    }
                                    else
                                    {
                                        DebugEx.Assert("Should not be here.");
                                    }
                                }
                                else
                                    DebugEx.Assert("Should not be here.");
                            }
                            return RemoveGraphIOsConns(realConnections);
                        }
                        else
                            return RemoveGraphIOsConns(realConnections);
                    }
                    else
                        return RemoveGraphIOsConns(Connections);
                }
                catch (Exception ex)
                {
                    DebugEx.TraceErrorException(ex);
                    return null;
                }
            }
        }

        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public List<AnnotationDescriptor> Annotations;

        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public List<LinkPointDescriptor> LinkPoints;

        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public List<GraphIODescriptor> Inputs;

        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public List<GraphIODescriptor> Outputs;

        #region Macro block info

        [DB_IgnoreIfDefault]
        public bool HasMacroBlock { get; set; }

        [DB_IgnoreIfDefault]
        public String MacroBlockName;

        [DoNotStoreInDB]
        public static readonly string MacroBlockType = "MacroBlock";

        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public String Description;

        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public String FriendlyImageSource;

        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public String Hierarchy;

        #endregion

        [DB_IgnoreIfDefault]
        public String FriendlyName;

        [DB_IgnoreIfDefault]
        public String Path;

        [DB_IgnoreIfDefault]
        public String GraphKey; //not sure if needed

        [DB_IgnoreIfDefault]
        public DateTime CreatedTimestamp;

        [DB_IgnoreIfDefault]
        public DateTime UpdatedTimestamp;

        public HashSetTS<BinaryResourceDescriptorKey> BackgroundImageKeys = new HashSetTS<BinaryResourceDescriptorKey>();

        // keep maximum width and height of background images
        [DB_IgnoreIfDefault]
        public int Width = 2000;

        [DB_IgnoreIfDefault]
        public int Height = 2000;

        [DB_IgnoreIfDefault]
        public int BackgroundImageWidth;

        [DB_IgnoreIfDefault]
        public int BackgroundImageHeight;

        [DB_IgnoreIfDefault]
        public float Zoom = 0.0f;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        /// <summary> This is for json deserialize </summary>
        public GraphDescriptor()
        {
            Connections = new List<ConnectionDescriptor>();
            Blocks = new List<BlockDescriptor>();
            Annotations = new List<AnnotationDescriptor>();
            LinkPoints = new List<LinkPointDescriptor>();
            Inputs = new List<GraphIODescriptor>();
            Outputs = new List<GraphIODescriptor>();
            CreatedTimestamp = DateTime.UtcNow;
            UpdatedTimestamp = DateTime.UtcNow;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //-------------------------------------------------------------------------------------------------------------------------
        public bool FunctionallyEqual(GraphDescriptor other)
        {
            //if (FriendlyName != other.FriendlyName)
            //    return false;
            if (GraphKey != other.GraphKey || Path != other.Path)
                return false;
            if (Blocks.Count != other.Blocks.Count || Connections.Count != other.Connections.Count)
                return false;
            foreach (var block in Blocks)
            {
                var match = other.Blocks.Find(x => x.UID == block.UID);
                if (match == null || !block.FunctionallyEqual(match))
                    return false;
            }
            return Connections.All(connection => other.Connections.Find(x => x.FromID == connection.FromID && x.ToID == connection.ToID) != null);
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public GraphDescriptor ShallowClone()
        {
            return this.MemberwiseClone() as GraphDescriptor;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        private bool _LinkPointsHelpers(out List<ConnectionDescriptor> connections2LinkPoints, out List<ConnectionDescriptor> connectionsFromLinkPoints)
        {
            connections2LinkPoints = new List<ConnectionDescriptor>();
            connectionsFromLinkPoints = new List<ConnectionDescriptor>();

            if (LinkPoints == null || LinkPoints.Count == 0)
                return true;

            // get all blocks' uids
            List<int> blockUIDs = Blocks?.Select(x => x.UID).ToList();
            // get graph's inputs/outputs uids (if any)
            var graphIOsUIDs = Inputs.Concat(Outputs).Select(x => x.UID)?.ToList();

            // get all connections from Block to Block
            var block2BlockConnections = Connections.FindAll(x => blockUIDs.Contains(x.FromID) && blockUIDs.Contains(x.ToID));
            // get all connections with graph io elements
            var graphIOWithBlockConns = Connections.FindAll(x => (graphIOsUIDs.Contains(x.FromID) && blockUIDs.Contains(x.ToID))
            || (graphIOsUIDs.Contains(x.ToID) && blockUIDs.Contains(x.FromID)));
            // non real connections are those which contains link points
            var nonRealConnections = Connections.Except(block2BlockConnections.Concat(graphIOWithBlockConns))?.ToList();
            // GraphDescriptor contains not connected link points?
            if (nonRealConnections == null && nonRealConnections.Count == 0)
                return false;

            // get all connections to link points
            connections2LinkPoints = nonRealConnections.FindAll(x => (blockUIDs.Contains(x.FromID) && !blockUIDs.Contains(x.ToID))
            || graphIOsUIDs.Contains(x.FromID) && !graphIOsUIDs.Contains(x.ToID))?.ToList();
            // GraphDescriptor contains not connected link points?
            if (connections2LinkPoints == null && connections2LinkPoints.Count == 0)
                return false;

            // get all connections from link points
            connectionsFromLinkPoints = nonRealConnections.FindAll(x => (blockUIDs.Contains(x.ToID) && !blockUIDs.Contains(x.FromID))
            || graphIOsUIDs.Contains(x.ToID) && !graphIOsUIDs.Contains(x.FromID))?.ToList();
            // GraphDescriptor has a non even count of connections between link points?
            if (connections2LinkPoints.Count != connectionsFromLinkPoints.Count)
                return false;

            return true;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public List<ConnectionDescriptor> RemoveGraphIOsConns(List<ConnectionDescriptor> connections)
        {
            // check that there graph io elements
            if (Inputs.Count == 0 && Outputs.Count == 0)
                return connections;
            // get graph's inputs/outputs uids
            var graphIOsUIDs = Inputs.Concat(Outputs).Select(x => x.UID)?.ToList();
            // find and remove connections with graph io elements
            var graphIOConnections = connections.FindAll(x => graphIOsUIDs.Contains(x.FromID) || graphIOsUIDs.Contains(x.ToID));
            // return inner connections for graph descriptor
            return connections.Except(graphIOConnections)?.ToList();
        }

        #endregion
    }
}
