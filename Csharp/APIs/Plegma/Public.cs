using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Yodiwo.API.Plegma
{

    #region API message types and base class

    // -------------------------------------------------------------------------------
    //  API message IDs and base class
    // -------------------------------------------------------------------------------

    /// <summary>
    /// Base class of an API message, from which all message classes inherit
    /// </summary>
    public abstract class ApiMsg
    {
        /// <summary>
        /// Sequence number of this message
        /// </summary>
        public int SeqNo;

        /// <summary>
        /// API Message constructor
        /// </summary>
        public ApiMsg() { }
    }

    public class GenericRsp : ApiMsg
    {
        /// <summary> Indicate that the requested action was successful or not  </summary>
        public bool IsSuccess;

        /// <summary> An optional code for result </summary>
        public int StatusCode;

        /// <summary> An optional message for the result </summary>
        public string Message;

        public GenericRsp() { }
    }
    #endregion


    #region Login-related message types

    /// <summary>
    /// Login Request
    /// to be used only for transports that require explicit authentication via the API itself
    /// Direction: Cloud to Node
    /// </summary>
    public class LoginReq : ApiMsg
    {
        /// <summary>
        /// constructor for login request
        /// </summary>
        public LoginReq()
            : base()
        {
        }
    }

    /// <summary>
    /// Connection parameters / flags
    /// </summary>
    [Flags]
    public enum eConnectionFlags
    {
        CreateNewEndpoint = 1 << 0,

        IsMasterEndpoint = 1 << 1
    }

    /// <summary>
    /// Login Response
    /// <para>sends node and secret keys</para>
    /// to be used only for transports that require explicit authentication via the API itself
    /// Direction: Node to Cloud
    /// </summary>
    public class LoginRsp : ApiMsg
    {
        /// <summary>
        /// NodeKey of Node
        /// </summary>
        public string NodeKey;
        /// <summary>
        /// Secret key of Node
        /// </summary>
        public string SecretKey;

        /// <summary>
        /// Flags that specify connection parameters
        /// </summary>
        public eConnectionFlags Flags;

        /// <summary>
        /// If creation of new endpoint isn't requested, then this variable can be used to request 
        /// that new Link is assigned to a specific Endpoint.
        /// This is handled as a suggestion and the server isn't obliged to comply
        /// </summary>
        public string DesiredEndpoint;

        /// <summary>
        /// parameterless constructor
        /// </summary>
        public LoginRsp()
            : base()
        {
        }
    }

    #endregion


    #region Node Types and API Messages

    // -------------------------------------------------------------------------------
    //  Node Types and API Message Classes
    // -------------------------------------------------------------------------------

    #region types

    /// <summary>
    /// Type of Node
    /// </summary>
    public enum eNodeType : byte
    {
        Unknown = 0,
        Gateway,
        EndpointSingle,

        TestGateway,
        TestEndpoint,
        WSEndpoint,
        Android,

        //Samples
        WSSample = 200,
        //XXX = 201
    };

    /// <summary>
    /// enum of possible node capabilites
    /// </summary>
    [Flags]
    public enum eNodeCapa : uint
    {
        /// <summary>no capabilities</summary>
        None = 0,

        /// <summary>Node supports graph solving</summary>
        SupportsGraphSolving = 1 << 0,
    }

    #region Node Thing Type defs

    public class StateDescription
    {
        /// <summary>
        /// Minimum value
        /// </summary>
        public double Minimum;
        /// <summary>
        /// Maximum value 
        /// </summary>
        public double Maximum;
        /// <summary>
        /// Change step size
        /// </summary>
        public double Step;
        /// <summary>
        /// Pattern to display (can be null)
        /// </summary>
        public string Pattern;
        /// <summary>
        /// Specifies whether the state is read only
        /// </summary>
        public bool ReadOnly;
        /// <summary>
        /// Specifies the state type
        /// </summary>
        public ePortType Type;
    }
    /// <summary>
    /// Describes restrictions and gives information of a configuration parameter.
    /// </summary>
    public class ConfigDescription
    {
        /// <summary>
        /// The default value (can be null)
        /// </summary>
        public string DefaultValue;
        /// <summary>
        /// Human readable description (can be null)
        /// </summary>
        public string Description;
        /// <summary>
        /// Human readable label (can be null or empty)
        /// </summary>
        public string Label;
        /// <summary>
        /// Name of the configuration parameter (must neither be null nor empty)
        /// </summary>
        public string Name;
        /// <summary>
        /// Specifies whether the parameter is required
        /// </summary>
        public bool Required;
        /// <summary>
        /// The data type of the parameter (can be null)
        /// </summary>
        public string Type;
        /// <summary>
        /// Minimum value
        /// </summary>
        public double Minimum;
        /// <summary>
        /// Maximum value 
        /// </summary>
        public double Maximum;
        /// <summary>
        /// Change step size
        /// </summary>
        public double Stepsize;
        /// <summary>
        /// Specifies whether the parameter is read only
        /// </summary>
        public bool ReadOnly;
    }
    /// <summary>
    /// Describes restrictions and gives information of a port <see cref="Port"/>.
    /// </summary>
    public class PortDescription
    {
        /// <summary>
        /// Human readable description for this port (can be null)
        /// </summary>
        public string Description;
        /// <summary>
        /// The unique identifier which identifies this port (must neither be null, nor empty)
        /// </summary>
        public string Id;
        /// <summary>
        /// Human readable label (can be null)
        /// </summary>
        public string Label;
        /// <summary>
        /// the category of this port , e.g. "TEMPERATURE" 
        /// </summary>
        public string Category;
        /// <summary>
        /// Describes the state of this port<see cref="StateDescription"/>
        /// </summary>
        public StateDescription State;
    }
    /// <summary>
    /// Base class that describes a Model of a Thing <see cref="Thing"/>
    /// </summary>
    public class ThingModelType
    {
        /// <summary>
        /// The unique identifier which identifies this model (must be neither null nor empty)
        /// </summary>
        public string Id;
        /// <summary>
        /// Human readable name for this model
        /// </summary>
        public string Name;
        /// <summary>
        /// Human readable description for this model
        /// </summary>
        public string Description;
        /// <summary>
        /// Describes the configuration parameter(s) of this model<see cref="ConfigDescription"/>
        /// </summary>
        public ConfigDescription[] Config;
        /// <summary>
        /// Describes the port(s) of this model<see cref="PortDescription"/>
        /// </summary>
        public PortDescription[] Port;
    }
    /// <summary>
    /// Base class that describes a group of Thing Models <see cref="ThingModelType"/>
    /// </summary>
    public class ThingType : IEquatable<ThingType>
    {
        /// <summary>
        /// The unique Type Name which identifies this group (must neither be null, nor empty)
        /// </summary>
        public string Type;
        /// <summary>
        /// Specifies whether model(s) of this group can automatically be discovered
        /// </summary>
        public bool Searchable;
        /// <summary>
        /// Human readable description for this group
        /// </summary>
        public string Description;
        /// <summary>
        /// Describes the model(s) of this group<see cref="ThingModelType"/>
        /// </summary>
        public ThingModelType[] Model;

        #region Equality

        public override int GetHashCode()
        {
            int hash = 0;
            //hash += NodeKey.GetHashCode();
            hash += (Type == null) ? 0 : Type.GetHashCode();
            hash += (Description == null) ? 0 : Description.GetHashCode();

            return hash;
        }

        public bool Equals(ThingType other)
        {
            if (other == null) { return false; }
            return (this.Type == other.Type);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ThingType)) { return false; }

            var tt = obj as ThingType;
            return Equals(tt);
        }

        public static bool operator ==(ThingType left, ThingType right) { return (object.ReferenceEquals(left, null) ? object.ReferenceEquals(right, null) : left.Equals(right)); }
        public static bool operator !=(ThingType left, ThingType right) { return !(object.ReferenceEquals(left, null) ? object.ReferenceEquals(right, null) : left.Equals(right)); }

        #endregion

    }
    #endregion

    #endregion

    #region message classes

    #region NodeInfo

    /// <summary>
    /// Node Info Request
    /// Sent by cloud to a node, it is to request capabilities and supported types from the node
    /// <para>Direction: Cloud->Node</para>
    /// <para>Node must reply with a <see cref="NodeInfoRsp"/></para>
    /// </summary>
    public class NodeInfoReq : ApiMsg
    {
        /// <summary>
        /// Informs of latest Plegma API Revision
        /// </summary>
        public int LatestApiRev;

        /// <summary>
        /// Endpoint that the node link used to send this message belongs to
        /// </summary>
        public string AssignedEndpoint;

        /// <summary>
        /// Node Info request constructor
        /// </summary>
        public NodeInfoReq()
            : base()
        {
            LatestApiRev = PlegmaAPI.APIVersion;
        }

        /// <summary>
        /// Revision number of Cloud server's entry for Node's Things
        /// </summary>
        public int ThingsRevNum;

        /// <summary>
        /// Node Info request constructor
        /// </summary>
        public NodeInfoReq(int seqNo)
            : this()
        {
            this.SeqNo = seqNo;
        }
    }

    /// <summary>
    /// Node Info Response
    /// Message that contains general information about a node including supported Node Types and Capabilities
    /// <para>Direction: bidirectional (Node->Cloud and Cloud->Node)</para>
    /// In response to a <see cref="NodeInfoReq"/>
    /// </summary>
    public class NodeInfoRsp : ApiMsg
    {
        /// <summary>
        /// Friendly name of responding Node
        /// </summary>
        public string Name;

        /// <summary>
        /// Type (<see cref="eNodeType"/>) of responding Node
        /// </summary>
        public eNodeType Type;

        /// <summary>
        /// Capabilities of this node
        /// </summary>
        public eNodeCapa Capabilities;

        /// <summary>
        /// List of <see cref="ThingType"/>s that this Node presents and implements
        /// </summary>
        public ThingType[] ThingTypes;

        /// <summary>
        /// Revision number of responding Node's Things
        /// </summary>
        public int ThingsRevNum;

        /// <summary>
        /// List of BlockLibraries that this Node supports
        /// </summary>
        public string[] BlockLibraries;

        /// <summary>
        /// Node Info Response constructor
        /// </summary>
        public NodeInfoRsp()
            : base()
        {
        }

        /// <summary>
        /// Node Info Response constructor
        /// </summary>
        public NodeInfoRsp(int seqNo)
            : base()
        {
            this.SeqNo = seqNo;
        }
    }

    #endregion

    #region NodeUnpairedReq/Rsp

    /// <summary>
    /// Reason for Node Unpairing
    /// </summary>
    public enum eUnpairReason : byte
    {
        /// <summary> Unknown Reason </summary>
        Unknown = 0,
        /// <summary> Node performed an invalid operation </summary>
        InvalidOperation = 1,
        /// <summary> User requested removal of the node from the Cyan UI </summary>
        UserRequested = 2,
        /// <summary> Node has failed to login too many times </summary>
        TooManyAuthFailures = 3
    };

    /// <summary>
    /// Unpairing request, stating reason code and a possible custom message
    /// Direction: Cloud->Node
    /// </summary>
    public class NodeUnpairedReq : ApiMsg
    {
        /// <summary> Reason code of unpairing </summary>
        public eUnpairReason ReasonCode;
        /// <summary> Custom unpair message (non-mandatory) </summary>
        public string Message;

        /// <summary>
        /// Node Unpaired message constructor
        /// </summary>
        public NodeUnpairedReq() : base()
        {
        }

        /// <summary>
        /// Node Unpaired message constructor
        /// </summary>
        public NodeUnpairedReq(int seqNo)
            : base()
        {
            this.SeqNo = seqNo;
        }
    }

    /// <summary>
    /// Unpairing Response. 
    /// Allowed to be empty, exists to make sure that node does receive message before being forcefully disconnected
    /// </summary>
    public class NodeUnpairedRsp : ApiMsg { }

    #endregion

    #region EndpointSyncReq/Rsp

    /// <summary>
    /// type of sync operation requested
    /// </summary>
    public enum eNodeSyncOperation
    {
        /// <summary>
        /// Node link endpoint asks for all available NodeEndpoints of Node
        /// </summary>
        GetEndpoints = 1,

        /// <summary>
        /// Node link requests that it is reassigned to new NodeEndpoint, denoted by <see cref="EndpointSyncReq.DesiredEndpoint"/>
        /// </summary>
        SetEndpoint
    }

    /// <summary>
    /// Endpoint Sync request, providing way for individual Node Links to become aware / influence Node operation
    /// Direction: Node(link) -> Cloud
    /// </summary>
    public class EndpointSyncReq : ApiMsg
    {
        /// <summary>
        /// type of sync operation requested
        /// </summary>
        public eNodeSyncOperation op;

        /// <summary>
        /// If <see cref="eNodeSyncOperation"/> is <see cref="eNodeSyncOperation.SetEndpoint"/>, then this variable can be used 
        /// to request that current Link is assigned/transferred to specific Endpoint.
        /// This is handled as a suggestion and the server isn't obliged to comply
        /// </summary>
        public string DesiredEndpoint;

        /// <summary>
        /// EndpointSync Request constructor
        /// </summary>
        public EndpointSyncReq() : base() { }

        /// <summary>
        /// EndpointSync Request constructor
        /// </summary>
        public EndpointSyncReq(int seqNo) : base()
        {
            this.SeqNo = seqNo;
        }
    }

    /// <summary>
    /// Endpoint Sync response to previous request
    /// Direction: Cloud -> Node(link)
    /// </summary>
    public class EndpointSyncRsp : ApiMsg
    {
        /// <summary>
        /// operation being replied to
        /// </summary>
        public eNodeSyncOperation op;

        /// <summary>
        /// Array of existing Endpoints currently assigned to NodeKey
        /// </summary>
        public string[] Endpoints;

        /// <summary>
        /// whether incoming Req was successful / accepted or not
        /// </summary>
        public bool Accepted;

        /// <summary>
        /// EndpointSync Response constructor
        /// </summary>
        public EndpointSyncRsp() : base() { }

        /// <summary>
        /// EndpointSync Response constructor
        /// </summary>
        public EndpointSyncRsp(int seqNo) : base()
        {
            this.SeqNo = seqNo;
        }
    }

    #endregion

    #endregion

    #endregion


    #region Thing handling API Message Classes

    // -------------------------------------------------------------------------------
    // Thing handling API Message Classes
    // -------------------------------------------------------------------------------

    /// <summary>
    /// Internal operation ID for <see cref="ThingsGet"/> and <see cref="ThingsSet"/> messages
    /// </summary>
    public enum eThingsOperation : byte
    {
        /// <summary>invalid opcode</summary>
        Invalid = 0,

        /// <summary>referenced things are to be updated at receiver. If they don't already exist, they are created</summary>
        Update = 1,

        /// <summary>
        /// referenced things are to be updated at receiver if they exist, created if not. 
        /// Previously existing things at receiver that are not in this message are *deleted*
        /// </summary>
        Overwrite = 2,

        /// <summary>ask that the receiver deletes referenced (by the ThingKey) thing</summary>
        Delete = 3,

        /// <summary>ask that receiver sends back its existing things as a <see cref="ThingsSet"/></summary>
        Get = 4,

        /// <summary>ask that the receiver scans for new things and send back all results (new and old) as a <see cref="ThingsSet"/></summary>
        Scan = 5,

        /// <summary>sync Thing revisions between node and cloud</summary>
        Sync = 6,
    }

    /// <summary>
    /// Node Things Request
    /// Used to request a <see cref="Thing"/>s related operation from the other end.
    /// <para>
    /// Receiving side *must* reply with a <see cref="ThingsSet"/>. 
    /// </para>
    /// <para>Direction: bidirectional (Node->Cloud and Cloud->Node)</para>
    /// </summary>
    public class ThingsGet : ApiMsg
    {
        /// <summary>
        /// Identifier of the operation requested; see <see cref="eThingsOperation"/>
        /// </summary>
        public eThingsOperation Operation;

        /// <summary>
        /// <see cref="ThingKey"/> of the <see cref="Thing"/> that this request refers to. If left null (invalid ThingKey)
        /// then this operation refers to all of the Node's Things
        /// </summary>
        public string ThingKey;

        /// <summary>
        /// Things revision number of sender; 0 if not available or applicable
        /// </summary>
        public int RevNum;

        /// <summary>
        /// Things Request constructor
        /// </summary>
        public ThingsGet()
            : base()
        {
        }

        /// <summary>
        /// Things Request constructor
        /// </summary>
        public ThingsGet(int seqNo)
            : this()
        {
            this.SeqNo = seqNo;
        }
    }

    /// <summary>
    /// Node Things Response
    /// Response to a <see cref="ThingsReq"/> request
    /// <para>
    /// a ThingsRsp message should have:
    ///  - <see cref="ThingsRsp.Operation"/> set to ThingReq's operation
    ///  - <see cref="ThingsRsp.Status"/> set to True if ThingsReq was successfully handled and this Msg has valid data, False otherwise
    ///  - if <see cref="ThingsRsp.Status"/> is True, <see cref="ThingsRsp.Data"/> set to correspond to requested Req's operation, set to Null otherwise. 
    ///    <see cref="ThingsRsp.Data"/> is allowed to be null if originally requested operation does not expect back data, only status
    /// </para>
    /// <para>Direction: bidirectional (Node->Cloud and Cloud->Node)</para>
    /// </summary>
    public class ThingsSet : ApiMsg
    {
        /// <summary>
        /// Identifier of this message's operation of type <see cref="eThingsOperation"/>
        /// Operation fields must match between Req and Rsp.
        /// </summary>
        public eThingsOperation Operation;

        /// <summary>
        /// Indicates if the request was successful and this response contains actual data
        /// </summary>
        public bool Status;

        /// <summary>
        /// Array of <see cref="Thing"/>s that contain data related to the selected Operation, if applicable
        /// </summary>
        public Thing[] Data; // optional data needed

        /// <summary>
        /// Things revision number of responder to a previous request; can be 0 if not available or applicable
        /// </summary>
        public int RevNum;

        /// <summary>
        /// Things Message constructor (for asynchronous messages)
        /// </summary>
        public ThingsSet()
            : base()
        {
        }
    }

    #endregion


    #region Ping Handling API Messages
    /// <summary>
    /// Node Ping Request
    /// </summary>


    public class PingReq : ApiMsg
    {
        /// <summary> A random number to verify the response </summary>
        public int Data;

        /// <summary>
        /// Ping Request constructor
        /// </summary>

        public PingReq()
            : base()
        {
        }
    }

    // -------------------------------------------------------------------------------

    public class PingRsp : ApiMsg
    {
        /// <summary> The data from the ping request </summary>
        public int Data;

        /// <summary>
        /// Ping Response constructor
        /// </summary>
        public PingRsp()
            : base()
        {
        }
    }
    #endregion


    #region Port handling, message&event passing API Message Classes

    // -------------------------------------------------------------------------------
    // Port handling, message&event passing API Message Classes
    // -------------------------------------------------------------------------------

    #region port event passing

    /// <summary>
    /// Port Event class: used to describe a new event that should trigger an endpoint, either towards a node or the Cloud Services
    /// </summary>
    public class PortEvent
    {
        /// <summary>
        /// <see cref="PortKey"/> of the <see cref="Port"/> this message refers to (either generating the event, or receiving the event)
        /// </summary>
        public string PortKey;

        /// <summary>
        /// Contents of the event in string form. See <see cref="Port.State"/>
        /// </summary>
        public string State;

        /// <summary>
        /// Revision number of this update; matches the Port State's internal sequence numbering. See <see cref="Port.State"/>
        /// </summary>
        public int RevNum;

        /// <summary>
        /// Timestamp (in msec since Unix Epoch) of event creation
        /// </summary>
        public ulong Timestamp;

        /// <summary>
        /// parameterless constructor for Port Events
        /// </summary>
        public PortEvent() { }

        /// <summary>
        /// constructor of PortEvent classes
        /// </summary>
        /// <param name="pkey">PortKey of Port this event refers to</param>
        /// <param name="state">State of Port (i.e. contents of message, <see cref="Port.State"/></param>
        /// <param name="revNum">Revision number of this event</param>
        /// <param name="timestamp">timestamp of this event in msec since Unix Epoch</param>
        public PortEvent(string pkey, string state, int revNum = 0, ulong timestamp = 0)
        {
            PortKey = pkey;
            State = state;
            RevNum = revNum;
            Timestamp = timestamp == 0 ? DateTime.UtcNow.ToUnixMilli() : timestamp;
        }
    }

    /// <summary>
    /// asynchronous Port Event message
    /// The main API message to exchange events between Nodes and the Yodiwo Cloud Service
    /// <para>Direction: bidirectional (Node->Cloud and Cloud->Node)</para>
    /// </summary>
    public class PortEventMsg : ApiMsg
    {
        /// <summary>
        /// Array of <see cref="PortEvent"/> messages
        /// </summary>
        public PortEvent[] PortEvents;

        /// <summary>
        /// parameterless constructor of PortEventMsgs
        /// </summary>
        public PortEventMsg()
            : base()
        {
        }

        /// <summary>
        /// constructor of PortEventMsgs with sequence numbers
        /// </summary>
        public PortEventMsg(int seqNo)
            : this()
        {
            this.SeqNo = seqNo;
        }

        /// <summary>
        /// quick(er) constructor for a single-event PortEventMsg
        /// </summary>
        /// <param name="seqNo">sequence number of this message</param>
        /// <param name="ev"></param>
        public PortEventMsg(int seqNo, PortEvent ev)
            : this(seqNo)
        {
            PortEvents = new PortEvent[] { ev };
        }

        public override string ToString()
        {
            return "PortEvent{" + PortEvents == null ? "null" : string.Join<PortEvent>(", ", PortEvents) + "}";
        }
    }

    /// <summary>
    /// VirtualBlock Event class: used to describe a new event that should trigger a virtual block endpoint, either towards a node or the Cloud Services
    /// </summary>
    public class VirtualBlockEvent
    {
        /// <summary>
        /// <see cref="BlockKey"/> of the Block this message refers to (either generating the event, or receiving the event)
        /// </summary>
        public string BlockKey;

        /// <summary>
        /// Block IO indices of the event.
        /// </summary>
        public byte[] Indices;

        /// <summary>
        /// Contents of the event in json string form.
        /// </summary>
        public string[] Values;

        /// <summary>
        /// Revision number of this update;
        /// </summary>
        public UInt64 RevNum;
    }

    /// <summary>
    /// asynchronous Port Event message
    /// The main API message to exchange events between Nodes and the Yodiwo Cloud Service
    /// <para>Direction: bidirectional (Node->Cloud and Cloud->Node)</para>
    /// </summary>
    public class VirtualBlockEventMsg : ApiMsg
    {
        /// <summary>
        /// Array of <see cref="PortEvent"/> messages
        /// </summary>
        public VirtualBlockEvent[] BlockEvents;

        /// <summary>
        /// parameterless constructor of PortEventMsgs
        /// </summary>
        public VirtualBlockEventMsg()
            : base()
        {
        }

        /// <summary>
        /// constructor with sequence numbers
        /// </summary>
        public VirtualBlockEventMsg(int seqNo)
            : this()
        {
            this.SeqNo = seqNo;
        }

        /// <summary>
        /// quick(er) constructor for a single-event PortEventMsg
        /// </summary>
        /// <param name="seqNo">sequence number of this message</param>
        /// <param name="ev"></param>
        public VirtualBlockEventMsg(int seqNo, VirtualBlockEvent ev)
            : this(seqNo)
        {
            BlockEvents = new VirtualBlockEvent[] { ev };
        }

        public override string ToString()
        {
            return "BlockEvent{" + BlockEvents == null ? "null" : string.Join<VirtualBlockEvent>(", ", BlockEvents) + "}";
        }
    }
    #endregion

    // -------------------------------------------------------------------------------

    #region Port state management

    /// <summary>
    /// Allowed operations in <see cref="PortStateReq"/> messages
    /// </summary>
    public enum ePortStateOperation
    {
        /// <summary>reserved; should not be used</summary>
        Invalid = 0,
        /// <summary>request array of current state for the specified PortKey(s)</summary>
        SpecificKeys = 1,
        /// <summary>request array of current states for ports currently deployed in graphs</summary>
        ActivePortStates = 2,
        /// <summary>request array of current states for all ports of this Node</summary>
        AllPortStates = 3,
    }

    /// <summary>
    /// Port State Request. Will result in a response of type <see cref="PortStateRsp"/>
    /// <para>Direction: node->cloud</para>
    /// </summary>
    public class PortStateReq : ApiMsg
    {
        /// <summary>Type of operation requested</summary>
        public ePortStateOperation Operation;

        /// <summary>
        /// List of PortKeys that the server should send an update for (in conjuction with <see cref="ePortStateOperation.SpecificKeys"/>). 
        /// If set to null or an empty array then the server will send an update for all relevant PortKeys
        /// </summary>
        public String[] PortKeys;

        /// <summary>Port update request constructor</summary>
        public PortStateReq()
            : base()
        {
        }
        /// <summary>
        /// <summary>Port update request constructor with seq.number</summary>
        /// </summary>
        /// <param name="seqNo">sequence number of this message</param>
        public PortStateReq(int seqNo)
            : this()
        {
            this.SeqNo = seqNo;
        }
        public override string ToString()
        {
            return "PortUpdateReq Op:" + Operation;
        }
    }

    /// <summary>
    /// internal state of a referenced Port
    /// </summary>
    public class PortState
    {
        /// <summary>
        /// <see cref="PortKey"/> of the <see cref="Port"/> this message refers to (either generating the event, or receiving the event)
        /// </summary>
        public string PortKey;

        /// <summary>
        /// Contents of port in string form. See <see cref="Port.State"/>
        /// </summary>
        public string State;

        /// <summary>
        /// Revision number of this update; matches the Port State's internal sequence numbering. See <see cref="Port.State"/>
        /// </summary>
        public int RevNum;

        /// <summary>
        /// Specifies whether this port is connected in currently deployed graphs
        /// </summary>
        public bool IsDeployed;

        /// <summary>
        /// parameterless constructor for Port States
        /// </summary>
        public PortState() { }

        /// <summary>
        /// constructor of PortEvent classes
        /// </summary>
        /// <param name="pkey">PortKey of Port this event refers to</param>
        /// <param name="state">State of Port (i.e. contents of message, <see cref="Port.State"/></param>
        /// <param name="revNum">Revision number of this event</param>
        /// <param name="isDeployed">Specifies whether this Port is currently active</param>
        public PortState(string pkey, string state, int revNum, bool isDeployed)
        {
            PortKey = pkey;
            State = state;
            RevNum = revNum;
            IsDeployed = isDeployed;
        }
    }

    // -------------------------------------------------------------------------------

    /// <summary>
    /// Active Port Keys Msg
    /// Informs Node of all currently active Ports (i.e. Ports that are connected and active in currently deployed graphs). 
    /// Should be used to 1. supress events from inactive ports, allowing more efficient use of medium, 2. sync Port states with the server
    /// <para>Direction: Cloud -> Node</para>
    /// </summary>
    public class PortStateRsp : ApiMsg
    {
        /// <summary>Type of operation responding to</summary>
        public ePortStateOperation Operation;

        /// <summary>
        /// Array of requested Port states.
        /// </summary>
        public PortState[] PortStates;

        /// <summary>Port update message constructor</summary>
        public PortStateRsp()
            : base()
        {
        }

        public override string ToString()
        {
            return "PortStateRsp{" + PortStates == null ? "null" : string.Join<PortState>(", ", PortStates) + "}";
        }
    }

    // -------------------------------------------------------------------------------
    /// <summary>
    /// Active Port Keys Msg
    /// Informs Node of all currently active Ports (i.e. Ports that are connected and active in currently deployed graphs). 
    /// Should be used by Nodes to supress events from inactive ports, allowing more efficient use of medium
    /// <para>Direction: Cloud -> Node</para>
    /// </summary>
    public class ActivePortKeysMsg : ApiMsg
    {
        /// <summary>
        /// Array of portkeys of currently active Ports
        /// </summary>
        public String[] ActivePortKeys;

        /// <summary>
        /// parameterless constructor of Active PortKeys message
        /// </summary>
        public ActivePortKeysMsg()
            : base()
        {
        }

        /// <summary>
        /// constructor of Active PortKeys message with sequence numbers
        /// </summary>
        public ActivePortKeysMsg(int seqNo)
            : this()
        {
            this.SeqNo = seqNo;
        }

        public override string ToString()
        {
            return "PortEventBatchMsg {" + ActivePortKeys == null ? "null" : string.Join<String>(",  ", ActivePortKeys) + "}";
        }
    }

    #endregion

    #endregion


    #region Graph Handling API Messages
    /// <summary>
    /// Inform server for local deployed graphs (to sync up on connect)
    /// </summary>
    public class LocallyDeployedGraphsMsg : ApiMsg
    {
        /// <summary> DeployedGraphKeys </summary>
        public string[] DeployedGraphKeys;
    }

    /// <summary>
    /// Node Graph Deploy/Undeploy Request (respond with <see cref="GenericRsp"/>)
    /// </summary>
    public class GraphDeploymentReq : ApiMsg
    {
        /// <summary> GraphKey </summary>
        public string GraphKey;

        /// <summary> IsDeployed </summary>
        public bool IsDeployed;

        /// <summary> GraphDescriptor </summary>
        public string GraphDescriptor;
    }
    #endregion


    #region A2MCU / A2mcu API Messages

    // -------------------------------------------------------------------------------
    public class A2mcuActiveDriver
    {
        public string BlockKey;
        public string ThingKey;
        public string DriverKey;
        public A2mcuSequencedCommands Init;
        public A2mcuSequencedCommands Deinit;
    }

    public class A2mcuActiveDriversReq : ApiMsg
    {
        public A2mcuActiveDriver[] ActiveDrivers;
    }
    // -------------------------------------------------------------------------------
    public abstract class A2mcuConcurrent
    {

    }

    public class A2mcuConcurrentCommands : A2mcuConcurrent
    {
        public IEnumerable<A2mcuCtrl> CtrlMsgs;
    }

    public class A2mcuSequencedCommands
    {
        public IEnumerable<A2mcuConcurrent> Seq;
    }
    // -------------------------------------------------------------------------------
    public enum eA2mcuCtrlType
    {
        Reset = 0,
        SetValue = 1,
        WriteDriconf = 2,
    }
    public class A2mcuCtrl : A2mcuConcurrent
    {
        public eA2mcuCtrlType Type;
        public string BlockKey;
        public string ThingKey;
        public object Data;
    }

    public class A2mcuCtrlReq : ApiMsg
    {
        public A2mcuCtrl[] CtrlMsgs;
    }

    #endregion
}
