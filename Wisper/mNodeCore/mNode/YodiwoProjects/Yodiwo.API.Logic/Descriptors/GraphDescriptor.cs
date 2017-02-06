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
                        realConnections.AddFromSource<ConnectionDescriptor>(block2BlockConnections);
                        // validate linkPoints

                        List<ConnectionDescriptor> block2LinkPointConnection, LinkPoint2BlockConnection;
                        if (_LinkPointsHelpers(out block2LinkPointConnection, out LinkPoint2BlockConnection))
                        {
                            foreach (var connection in block2LinkPointConnection)
                            {
                                // get input point
                                var inputPoint = LinkPoints.Find(x => x.UID == connection.ToID);
                                if (inputPoint != null)
                                {
                                    // get output point
                                    var outputPoint = LinkPoint2BlockConnection.Find(x => x.FromID == inputPoint.ConnectedPointUID);
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
                            return realConnections;
                        }
                        else
                            return realConnections;
                    }
                    else
                        // this graph contains no link points, all connections are 'real'
                        return Connections;
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
        public string FriendlyName;

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
        private bool _LinkPointsHelpers(out List<ConnectionDescriptor> block2LinkPointConnection, out List<ConnectionDescriptor> LinkPoint2BlockConnection)
        {
            block2LinkPointConnection = new List<ConnectionDescriptor>();
            LinkPoint2BlockConnection = new List<ConnectionDescriptor>();

            if (LinkPoints == null || LinkPoints.Count == 0)
                return true;

            // get all blocks' uids
            List<int> blockUIDs = Blocks?.Select(x => x.UID).ToList();

            // get all connections from Block to Block
            var block2BlockConnections = Connections.FindAll(x => blockUIDs.Contains(x.FromID) && blockUIDs.Contains(x.ToID));

            // non real connections are the difference between 'block2BlockConnections' and 'Connections'
            var nonRealConnections = Connections.Except<ConnectionDescriptor>(block2BlockConnections)?.ToList();
            // GraphDescriptor contains not connected link points?
            if (nonRealConnections == null && nonRealConnections.Count == 0)
                return false;

            // get all connections from block to linkpoint
            block2LinkPointConnection = nonRealConnections.FindAll(x => blockUIDs.Contains(x.FromID) && !blockUIDs.Contains(x.ToID))?.ToList();
            // GraphDescriptor contains not connected link points?
            if (block2LinkPointConnection == null && block2LinkPointConnection.Count == 0)
                return false;

            // get all connections from linkpoint to block
            LinkPoint2BlockConnection = nonRealConnections.Except<ConnectionDescriptor>(block2LinkPointConnection)?.ToList();
            // GraphDescriptor has a non even count of connections between link points?
            if (block2LinkPointConnection.Count != LinkPoint2BlockConnection.Count)
                return false;

            return true;
        }

        #endregion
    }
}
