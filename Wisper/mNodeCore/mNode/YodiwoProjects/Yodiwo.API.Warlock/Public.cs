using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Yodiwo.API.Logic;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Warlock
{
    #region API message types and base class

    // -------------------------------------------------------------------------------
    //  API message IDs and base class
    // -------------------------------------------------------------------------------

    /// <summary>
    /// Base class of an API message, from which all message classes inherit
    /// </summary>
    [Serializable]
    public abstract class WarlockApiMsg : Yodiwo.API.ApiMsg
    {
        public UserKey UserKey;
        public SubUserKey SubUserKey;
    }

    /// <summary>
    /// General Response to request-type messages. Used to unblock requests waiting for responses that are of basic ACKnowledge type
    /// </summary>
    [Serializable]
    public class GenericRsp : WarlockApiMsg
    {
        /// <summary>
        /// Marks whether the referenced by Id request was successful or not. A value of <see langword="false"/>
        /// means the request got handled but failed due to the reason provided in <see cref="Message"/>
        /// </summary>
        public bool IsSuccess;

        /// <summary> An optional code for result </summary>
        public int StatusCode;

        /// <summary> An optional message for the result </summary>
        public string Message;

        /// <summary>
        /// Generic response constructor
        /// </summary>
        public GenericRsp() { }
    }
    #endregion


    #region User Query API messages


    #region User tokens messages

#if false
    /// <summary>
    /// Request to (try to) convert emails/name to user tokens that can be used with further Warlock API requests
    /// For a successful conversion the requested user must either have allowed conversions from any source request,
    /// or they must have an allowed link with the requesting user
    /// </summary>
    public class GetUserTokensReq : WarlockApiMsg
    {
        /// <summary>
        /// Array of emails that Req tries to convert to user tokens
        /// </summary>
        public string[] Emails;

        /// <summary>
        /// Array of names that Req tries to convert to user tokens
        /// </summary>
        public string[] Names;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public GetUserTokensReq() : base() { }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Emails as IEnumerable<string>);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
#endif
    /// <summary>
    /// response to a <see cref="GetUserTokensReq"/>
    /// </summary>
    public class GetUserTokensRsp : WarlockApiMsg
    {
        /// <summary>
        /// source-key-to-token dictionary. Source key is the <see cref="GetUserTokensReq"/> entry
        /// that matches the returned token value.
        /// Tokens are transient and may stop working on further requests at any time, at which point 
        /// the API client is advised to request new ones via <see cref="GetUserTokensReq"/>
        /// 
        /// If multiple tokens match (i.e. there were duplicate requests) each one overwrites the previous one; so don't do that
        /// </summary>
        public Dictionary<string, string> Tokens;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public GetUserTokensRsp() : base()
        {
            Tokens = new Dictionary<string, string>();
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Tokens.Values as IEnumerable<string>);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    #endregion

    #region User Info messages

    /// <summary>
    /// User Information
    /// </summary>
    public class UserInfo
    {
        /// <summary>User Token. Can be used for all user-related queries and references</summary>
        public string UserToken;
        /// <summary>Username of user</summary>
        public string Username;
        /// <summary>Name of User as retrieved by User's selected authentication provider or set by the user themselves</summary>
        public string Name;
        /// <summary>Email of User as retrieved by User's selected authentication provider</summary>
        public string Email;
        /// <summary>Avatar of User as retrieved by User's selected authentication provider</summary>
        public string Avatar;
        /// <summary>True if user is currently logged in to the Cyan environment</summary>
        public bool LoggedIn;

        /// <summary>toString override</summary>
        public override string ToString()
        {
            return UserToken + ": " + Name + ", " + Email;
        }
    }

    public class LinkedUserInfo
    {
        public SubUserKey SubUserKey;
        public bool IsPrimary;
        public string Name;
        public string FirstName;
        public string LastName;
        public string Email;
        public string UserName;
        public string AvatarUri;
        public string TimeZoneId;
        public string Token;
        public string Locale;
        public bool LoggedIn;
        public DateTime CreatedTime;
        public DateTime UpdatedTime;
        public eUserPrivilegeLevel PrivilegeLevel;

    }

    /// <summary>
    /// response to a GetUserInfoReq
    /// </summary>
    public class GetUserInfoRsp : WarlockApiMsg
    {
        /// <summary>
        /// Dictionary Key: user token of user about which info is being provided
        /// Dictionary Value: user info as a <see cref="UserInfo"/> class
        /// </summary>
        public Dictionary<string, UserInfo> UserInfo;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public GetUserInfoRsp() : base()
        {
            UserInfo = new Dictionary<string, UserInfo>();
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    #endregion

    /// <summary>
    /// response to <see cref="GetFriendsReq"/>
    /// </summary>
    public class GetFriendsRsp : GenericRsp
    {
        /// <summary>
        /// list of user tokens that user is friends with
        /// </summary>
        public Dictionary<string, UserInfo> Friends;

#pragma warning disable CS1591
        public GetFriendsRsp() : base() { }
#pragma warning restore CS1591
    }

    #endregion

    #region Things messages
    /// <summary>
    /// Get Things response as a dictionary of node keys to lists of Things
    /// </summary>
    public class GetThingsRsp : GenericRsp
    {
        /// <summary>
        /// Dictionary Key: node key of the things referenced in Value
        /// Dictionary Value: list of Plegma API Things that belong to the node reference in the Key
        /// </summary>
        public Dictionary<string, List<ThingDescriptor>> ThingsPerNode;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public GetThingsRsp() : base() { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    #endregion


    #region Sharing Ctrl API messages

    /// <summary>
    /// Sharing Action type
    /// </summary>
    public enum eShareActionType
    {
        /// <summary>
        /// Explicit share request, must include specific ThingKey and specific User Id to share to
        /// </summary>
        Share,

        /// <summary>
        /// Explicit unshare request, must include specific ThingKey and either a specific User Id to unshare from,
        /// or a broadcast UserKey in order to unshare from all
        /// </summary>
        Unshare,

        /// <summary>
        /// Get sharing info for specified ThingKey. Specific ThingKey must be provided (no broadcast keys allowed). If
        /// specific TargetUser is provided the response will inform whether Thing is shared with specific User (via the <see cref="Yodiwo.API.Warlock.GenericRsp.IsSuccess"/> field).
        /// If broadcast UserKey is provided and the requesting User is the Thing's Owner, then the response will provide an array of users the Thing is currently shared to via <see cref="Yodiwo.API.Warlock.ShareActionRsp.Users"/>.
        /// A broadcast UserKey cannot be used to request the list of users for a Thing that doesn't belong to the requester.
        /// </summary>
        Ask,

        /// <summary>
        /// Used to receive two Things lists:
        /// - Things shared to the requesting user by other users (in <see cref="ShareActionRsp.ThingKeysByOthers"/>)
        /// - Things shared to other users by the requesting user (in <see cref="ShareActionRsp.ThingKeysToOthers"/>)
        /// </summary>
        AskAll,

        /// <summary>
        /// Used to deny pending share or unshare request. 
        /// Must include PendingToken and TargetUserToken
        /// </summary>
        Deny
    }

    /// <summary>
    /// Share Action sub-request
    /// </summary>
    public class ShareActionReq
    {
        /// <summary>
        /// Id of specific sub-request. Can be ignored if Req consists of single action or requester doesn't require to match requests to responses
        /// </summary>
        public int Id;

        /// <summary>
        /// Type of action requested; see <see cref="eShareActionType"/>
        /// </summary>
        public eShareActionType Type;

        /// <summary>
        /// User Token of Target User the action is referring to, as retrieved by <see cref="GetUserTokensReq"/>. A wildcard key may be used to request info on all users for a single ThingKey
        /// </summary>
        public string TargetUserToken;

        /// <summary>
        /// Specific ThingKey of Thing that this action refers to. Mandatory for <see cref="eShareActionType.Share"/>, <see cref="eShareActionType.Unshare"/> and <see cref="eShareActionType.Ask"/>.
        /// Ignored for <see cref="eShareActionType.AskAll"/>.
        /// </summary>
        public string ThingKey;

        /// <summary>
        /// If the request is used to answer a previously pending request, this token should be used to reference the initial request
        /// </summary>
        public string PendingToken;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
        {
            return "ID:" + Id + " / Type:" + Type + " / ThingKey:" + ThingKey + " / Target User:" + TargetUserToken;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    public class ShareGraphReq
    {
        /// <summary>
        /// Type of action requested <see cref="eShareActionType"/>
        /// </summary>
        public eShareActionType Type;

        /// <summary>
        /// Shared graph's key
        /// </summary>
        public GraphDescriptorKey GraphDescriptorKey;

        /// <summary>
        /// Email of user who made the request
        /// </summary>
        public string RequesterEmail;

        /// <summary>
        /// Notification's key
        /// </summary>
        public NotificationDescriptorKey NotificationDescriptorKey;
    }

    public class SharedGraphConfReq : GenericRsp
    {
        /// <summary>
        ///  Current wizard step
        /// </summary>
        public int Step;

        /// <summary>
        /// Data to send to warlock after step 1 submission
        /// </summary>
        public string GraphPath;
        public string GraphName;
        public bool OverwriteGraph;
        public GraphDescriptorKey GraphDescriptorKey;
        public NotificationDescriptorKey NotificationDescriptorKey;
        public string RequesterEmail;
        public string PendingToken;

        /// <summary>
        /// Data to send to warlock after step 2 submission
        /// </summary>
        public Dictionary<string, ThingKey> ThingType2ThingKey = new Dictionary<string, ThingKey>();
    }

    public class SharedGraphConfRsp : GenericRsp
    {
        /// <summary>
        /// Data to send to frontend to create view for step 2
        /// </summary>
        public HashSet<string> ThingTypesInSharedGraph = new HashSet<string>();
        public Dictionary<string, List<ThingKey>> ThingType2ThingKeys = new Dictionary<string, List<ThingKey>>();
        public Dictionary<ThingKey, ThingDescriptor> TargetThingKey2ThingDescriptor = new Dictionary<ThingKey, ThingDescriptor>();

        /// <summary>
        /// Errors found on current step
        /// </summary>
        public Dictionary<string, string> Errors = new Dictionary<string, string>();

        /// <summary>
        /// Flag to indicate if graph configuration has completed
        /// </summary>
        public bool IsFinished;
    }

    /// <summary>
    /// Share Action Response class. Each <see cref="ShareThingsRsp"/> message contains the same number of these as in 
    /// the <see cref="ShareThingsReq"/> message it is responding to, and each <see cref="ShareActionRsp.Id"/> corresponds to a <see cref="ShareActionReq.Id"/>
    /// </summary>
    public class ShareActionRsp : GenericRsp
    {
        /// <summary>
        /// Id of request that this response refers to
        /// </summary>
        public int Id;

        /// <summary>
        /// If true, the respective request was not completed because of an action pending on the receiver's part. In such a case <see cref="PendingToken"/>
        /// will be set to a value that will be provided as reference in a future <see cref="ShareNotifyMsg"/>
        /// </summary>
        public bool IsPending;

        /// <summary>
        /// A token that can be used to match a currently pending operation to a future <see cref="ShareNotifyMsg"/>
        /// </summary>
        public string PendingToken;

        /// <summary>
        /// Contains Things shared to other users by the requesting user. Used only when <see cref="ShareActionReq.Type"/> is <see cref="eShareActionType.AskAll"/>
        /// Dictionary Key is the User Token of the shared-to user, Value is the list of ThingKeys that are being shared to this User
        /// Dictionary Key is the ThingKey that is being shared toward other users, Value is the list of User Tokens of the users that the thing is being shared to
        /// </summary>
        public Dictionary<string, List<string>> ThingKeysToOthers;

        /// <summary>
        /// Contains Things shared to the requesting user by other users. Used only when <see cref="ShareActionReq.Type"/> is <see cref="eShareActionType.AskAll"/>
        /// Dictionary Key is the User Token of the shared-from users, Value is the list of that user's shared ThingKeys
        /// </summary>
        public Dictionary<string, List<string>> ThingKeysByOthers;

        /// <summary>
        /// (refers to <see cref="eShareActionType.Ask"/> requests only) Array of Users that referenced ThingKey is currently shared with. May be empty.
        /// <para>Is left empty for <see cref="eShareActionType.Share"/> and <see cref="eShareActionType.Unshare"/> requests where the request outcome
        /// is derived from the <see cref="GenericRsp.IsSuccess"/> and <see cref="GenericRsp.Message"/> fields</para>
        /// </summary>
        public string[] Users;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string ToString()
        {
            return "ID:" + Id + " / Success:" + IsSuccess + " / ThingKey:" + ThingKeysToOthers + " / Users:" + string.Join(",", Users);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// Response to a <see cref="ShareThingsReq"/> message
    /// </summary>
    public class ShareThingsRsp : WarlockApiMsg
    {
        /// <summary>
        /// indication of whether message as whole got handled or not;
        /// <para>
        /// <see langword="false"/> means that there was a premature error (e.g. bad request) and that the <see cref="ShareThingsRsp"/> field 
        /// contains no data.
        /// </para><para>
        /// <see langword="true"/> means that the <see cref="ShareThingsRsp"/> field contains valid data
        /// </para>
        /// </summary>
        public bool Handled;

        /// <summary>
        /// Array of sub-responses to each of <see cref="ShareThingsReq"/>'s sub-requests
        /// </summary>
        public ShareActionRsp[] ShareActionRsps;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ShareThingsRsp() : base() { }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, ShareActionRsps as IEnumerable<ShareActionRsp>);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// Notification message; may inform about:
    ///  - something that happened successfully and Warlock subscribers need to be informed (<see cref="PendingToken"/> is empty and <see cref="IsSuccessful"/> is true
    ///  - a share event that is pending and requires user action (<see cref="PendingToken"/> contains a token that can be used in a subsequent "answer" Share Request)
    /// </summary>
    public class ShareNotification
    {
        /// <summary>
        /// Type of action that is being notified; see <see cref="eShareActionType"/>
        /// </summary>
        public eShareActionType Type;

        /// <summary>
        /// If not empty, it holds a token that can be used to match a currently pending operation to a future <see cref="ShareNotifyMsg"/>
        /// </summary>
        public string PendingToken;

        /// <summary>
        /// If <see cref="PendingToken"/> not empty, this holds the outcome of previously-pending share action
        /// </summary>
        public bool IsSuccessful;

        /// <summary>
        /// Token of User that requested the action this is referring to
        /// </summary>
        public string RequestingUserToken;

        /// <summary>
        /// UserKey of Target User the action is referring to
        /// </summary>
        public string TargetUserToken;

        /// <summary>
        /// Specific ThingKey of Thing that this action refers to
        /// </summary>
        public string ThingKey;
    }

    /// <summary>
    /// Share notification message: sends one or more notifications about sharing events:
    ///  - either that something happened successfully and Warlock subscribers need to be informed
    ///  - or that a share event is pending and requires user action
    /// </summary>
    public class ShareNotifyMsg : WarlockApiMsg
    {
        /// <summary>
        /// Array of notification messages, each one specifying a separate, independent sharing notification towards the receiver
        /// </summary>
        public ShareNotification[] Notifications;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ShareNotifyMsg() : base() { }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Notifications as IEnumerable<ShareNotification>);
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    #endregion


    #region Graphs Ctrl API messages

    public enum eGraphActionType : byte
    {
        Invalid = 0,

        /// <summary>
        /// Explicit deploy request, must include specific GraphDescriptorKey
        /// </summary>
        Deploy,

        /// <summary>
        /// Explicit undeploy request, must include specific GraphDescriptorKey
        /// </summary>
        Undeploy,

        /// <summary>
        /// Validate selection request, must include GraphDescriptor in json
        /// </summary>
        Validate,

        /// <summary>
        /// Explicit load request, must include specific GraphDescriptorKey
        /// </summary>
        Load,

        /// <summary>
        /// Save graph request, must include GraphDescriptor in json 
        /// </summary>
        Save,

        /// <summary>
        /// Replace graph request, must include GraphDescriptor in json and 
        /// graph's GraphDescriptorKey that will be replaced
        /// </summary>
        Replace,

        Copy,
        Update,

        /// <summary>
        /// import graph request, must include GraphDescriptor in json
        /// </summary>
        Import,

        /// <summary>
        /// Explicit export request, must include specific GraphDescriptorKey
        /// </summary>
        Export,

        /// <summary>
        /// Explicit remove request, must include specific GraphDescriptorKey
        /// </summary>
        Remove,

        /// <summary>
        /// Explicit graph info request, must include specific GraphDescriptorKey
        /// Returns a GraphWrlkDescriptor <see cref="GraphWrlkDescriptor"/>
        /// </summary>
        Info,

        /// <summary>
        /// Share graph request, must include specific GraphDescriptorKey and TargetUserEmail
        /// </summary>
        Share,
    }

    public class GraphActionReq : WarlockApiMsg
    {
        /// <summary>
        /// Type of graph action; see <see cref="eGraphActionType"/>
        /// </summary>
        public Yodiwo.API.Warlock.eGraphActionType ActionType;

        /// <summary>
        /// Specific GraphDescriptorKey of GraphDescriptor see <see cref="Yodiwo.API.Warlock.GraphDescriptorAndGroupNotificationInfo.GraphDescriptorKey"/> that this action refers to
        /// </summary>
        public System.String GraphDescriptorKey;

        /// <summary>
        /// Specific data in json that this action refers to. 
        /// Can contain:
        ///     + GraphDescriptor see <see cref="GraphDescriptorKey"/>
        ///     + GraphActionUpdateReq see <see cref="GraphActionUpdateReq"/>
        /// </summary>
        public System.String JsonData;

        /// <summary>
        /// User's email to share to
        /// </summary>
        public string TargetUserEmail;
    }

    public class GraphActionUpdateReq
    {
        public string Name;
        public string Path;

        public bool IsValid()
        {
            return !Name.IsNullOrEmpty();
        }
    }

    public class GraphActionRsp : GenericRsp
    {
        /// <summary>
        /// Specific GraphDescriptorKey of GraphDescriptor see <see cref="Yodiwo.API.Warlock.GraphActionRsp.GraphDescriptorKey"/> that this action refers to
        /// </summary>
        public string GraphDescriptorKey;

        /// <summary>
        /// Specific data resulting from this action
        /// Can contain:
        ///     + GraphDescriptor see <see cref="Yodiwo.API.Warlock.GraphActionRsp.GraphDescriptorKey"/>
        ///     + GraphWrlkDescriptor see <see cref="GraphWrlkDescriptor"/>
        /// </summary>
        public string JsonData;

        /// <summary>
        /// If true, the respective request was not completed because of an action pending on the receiver's part
        /// </summary>
        public bool isPending;
    }

    public class MultiGraphActionRsp : GenericRsp
    {
        public GraphActionRsp[] Responses;
    }

    public class LiveKeysAddMsg : WarlockApiMsg
    {
        public string[] LivePortKeys;
    }

    public class LiveKeysDelMsg : WarlockApiMsg
    {
        public string[] LivePortKeys;
    }

    public class LiveValuesMsg : WarlockApiMsg
    {
        public LiveValue[] LiveValues;
    }

    public class LiveValue
    {
        public object Value;
        public int Index;
        public bool IsConnected;
        public bool IsDirty;
        public bool IsInput;
        public bool IsOutput;
        public bool IsTouched;
        public bool IsPort;

        public string IoName;
        public string BlockKey;
        public string PortKey;
        public string Extra;
        public uint RevNum;
        public ulong Timestamp;
    }

    #endregion


    #region Hackingstuff

    /// <summary>
    /// Just for Demo purposes (creating on-the-fly scenarios via node)
    /// </summary>
    public class MyHackingRsp : WarlockApiMsg
    {
        public string AppToken;

        public bool IsSuccess;

        public string Key;
        public MyHackingRsp() : base() { }
    }

    #endregion


    #region warlock-extra-capabilities
    //warlock extra capabilities

    public class ScanRsp : GenericValueST<string[]>
    {
    }

    public class ValidationResultDescriptor
    {
        public bool IsSuccess;
        public string Message;
    }

    public class RegisterUserKey
    {
        public HashSetTS<UserKey> Userkeys;
    }

    public class UnRegisterUserKey
    {
        public HashSetTS<UserKey> Userkeys;
    }

    public class GetSharedThingsRsp : GenericRsp
    {
        public List<ThingShareInfoDescriptor> SharedThings;
    }

    public class NodeDescriptorsRsp : GenericRsp
    {
        public List<NodeDescriptor> NodeDescriptors;
    }

    public class GroupsRsp : GenericRsp
    {
        public List<GroupDescriptor> GroupDescriptors;
    }

    public class NodeStatusRsp : GenericRsp
    {
        public eNodeStatus NodeStatus;
    }

    public class NodeCreateRsp : GenericRsp
    {
        public NodeKey Nodekey;
        public string Secret;
    }

    public class GetThingTypesRsp : GenericRsp
    {
        public List<ThingTypeDescriptor> ThingTypeDescriptors;
    }

    public class GetNotificationsRsp : GenericRsp
    {
        public List<NotificationDescriptor> NotificationDescriptors;
    }

    public class GenericValue : GenericRsp
    {
        public object Value;
    }

    public class GenericValueST<T> : GenericRsp
    {
        public T Value;
    }

    public class GetUpdatedUserDescRsp : GenericValueST<UserDescriptor> { };

    public class GetAllPortsRsp : GenericValueST<List<PortInformation>> { };
    public class GetAllPortKeysRsp : GenericValueST<List<PortKey>> { };
    public class GetUserDescriptorRsp : GenericValueST<UserDescriptor> { };
    public class GetLinkedUserDescriptor : GenericValueST<LinkedUserInfo> { };
    public class GetAllUserDescriptorsRsp : GenericValueST<List<UserDescriptor>> { };
    public class NodePairingRecoverRsp : GenericValueST<NodePairingModelView> { };
    public class GetToolboxModelViewRsp : GenericValueST<string> { };
    public class GetBlockDescriptorRsp : GenericValueST<string> { };
    public class GetUserQuotaRsp : GenericValueST<UserQuotaDescriptor> { };
    public class SetUserSettingsRsp : GenericValueST<UserDescriptor> { };
    public class SocialAccountRsp : GenericValueST<object> { };
    public class GetPortResp : GenericValueST<PortDescriptor> { };
    public class CreateApiKeyRsp : GenericValueST<ApiKeyDescriptor> { };
    public class CreateGroupRsp : GenericValueST<GroupKey> { };
    public class MultipleThingActionRsp : GenericValueST<Dictionary<ThingKey, GenericRsp>> { };

    public enum ThingAction
    {
        None,
        Hide,
        UnHide,
        Delete,
    }
    public class ValidationResDescriptor
    {
        public string Msg;
        public ResultType Type;

        public ValidationResDescriptor() { }

        public ValidationResDescriptor(string Msg, ResultType Type)
        {
            this.Msg = Msg;
            this.Type = Type;
        }
        public enum ResultType
        {
            Info,
            Warning,
            Error,
        }
    }

    public class DeployResponse : GenericRsp
    {
        public Dictionary<int, List<ValidationResDescriptor>> ValidationResults;
        public string GraphDescriptor;
    }


    public class DeployedGraphRevRsp : GenericRsp
    {
        public int LatestRev;
        public int DeployedRev;
    }

    public class BookmarkableLoadRoute : GenericRsp
    {
        public GraphDescriptorKey GraphDescriptorKey;
        public bool IsPlacer;
    }

    public class BinaryResourceDescRsp : GenericRsp
    {
        public IEnumerable<BinaryResourceDescriptor> Descriptors;
    }

    public class RestServiceRsp : GenericRsp
    {
        public List<RestServiceBlockDescriptor> RestServices;
    }

    public class SocialAccountsRsp : GenericRsp
    {
        public UserSocialAccountsDescriptor SocialAccounts;
    }

    public class ApiKeyRsp : GenericRsp
    {
        public List<ApiKeyDescriptor> ApiKeys;
    }

    public class OAuthGetTokenResponse : GenericRsp
    {
        public string Token;
        public string RedirectUrl;
    }

    public class DashboardGraphsResponse : GenericRsp
    {
        public long SavedGraphs;
        public long DeployedGraphs;
    }

    public class LoadGraphsResp : GenericRsp
    {
        public List<GraphWrlkDescriptor> GraphDescriptors;
    }

    public class LoadFilesResp : GenericValueST<List<BinaryResourceDescriptor>>
    {
    }

    public class NodePairingCompetionRsp : GenericRsp
    {
        public string RedirectUri;
        public System.Net.HttpStatusCode Status;
        public int AttemptsLeft;
        public string Instructions;
        public bool Clearsession;
    }

    public class UploadRequest
    {
        public BinaryResourceDescriptorKey BinaryResourceDescriptorKey;
        public string Guid;
        public string Uri;
        public string FriendlyName;
        public string FriendlyDescription;
        public long Size;
        public string HttpContentType;
        public string ContentDescriptorJson; // Json of ContentDescriptor
        public string LocationDescriptorJson; // Json of LocationDescriptor
    }

    public class GrafanaSnapshotCreateResponse : GenericRsp
    {
        public string DeleteKey;
        public string DeleteUrl;
        public string Key;
        public string Url;
    }

    public class UserLoginReq
    {
        public string Email;
        public string PasswordHash;
        public string Name;
        public bool IsDemoLogin;
    }

    public class UserLoginRsp : GenericRsp
    {
        public UserDescriptor UserDescriptor;
        public bool UserIsNew;
        public bool UserIsLocked;
    }

    public class UserDescriptor
    {
        public UserKey UserKey;
        public SubUserKey SubUserKey;
        public ePrivilegeLevel PrivelegeLevel;
        public string Name;
        public string FirstName;
        public string LastName;
        public string Email;
        public string AvatarUri;
        public RouteRequest Request;
        public string TimeZoneId;
        public UserOptions Options;
        public string Token;
        public int AccountLevel;
        public bool InMemory;
        public bool LoggedIn;
        public bool IgnoreQuota;
        public bool AccountLocked;

        #region UserDescriptor Constructors
        public UserDescriptor(UserKey usrKey)
        {
            UserKey = usrKey;
        }
        #endregion
    }



    public enum RouteRequest
    {
        Default,
        LoadGraph
    }

    public class BlockActionReq
    {
        public GraphDescriptorKey GraphDescriptorKey;
        public int BlockId;
        public string Message;
    }

    public class PortInformation : PortDescriptor, IEquatable<PortInformation>
    {
        public int Index;
        public string Value { get { return State; } }

        public PortInformation() { }

        public PortInformation(PortDescriptor port, int index)
        {
            this.PortKey = port.PortKey;
            this.Name = port.Name;
            this.Description = port.Description;
            this.IoDirection = port.IoDirection;
            this.PortType = port.PortType;
            this.Semantics = port.Semantics;
            this.State = port.State;
            this.RevNum = port.RevNum;
            this.LastUpdatedTimestamp = port.LastUpdatedTimestamp;
            this.HasRules = port.HasRules;
            this.Size = port.Size;
            this.Color = port.Color;
            this.Extra = port.Extra;
            this.IsInput = port.IsInput;
            this.IsOutput = port.IsOutput;
            this.Index = index;
        }

        public bool Equals(PortInformation other)
        {
            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            return other.PortKey == PortKey;
        }

        public override int GetHashCode()
        {
            return PortKey == null ? 0 : PortKey.GetHashCode();
        }

        /*

        public class LiveExecutionBlockSolve
        {
            public object Value;
            public int Index;
            public bool IsConnected;
            public bool IsDirty;
            public bool IsTouched;
            public string Extra;


        }

       
        public class LiveExecutionPortStates
        {
            public LiveExecutionPortState[] PortStates;
        
            */



    }

    public class GetPortRulesRsp : GenericValueST<Dictionary<string, string>>
    {
    }

    public class WorkerEventNotification
    {
        public EventMsgType MsgType;
        public object Data;
        public Yodiwo.API.Plegma.GenericRsp Response;
    }

    public enum LiveExecutionEventType
    {
        None,
        Solve,
        PortStates,
    }

    public enum LiveListenerType
    {
        None,
        Blocks,
        Graphs,
        Ports,
        All
    }

    public class LiveExecutionEvent
    {
        public LiveExecutionEventType LiveExecutionEvType;
        public LiveExecutionSolve LiveExecutionSolve;
        public LiveExecutionPortStates LiveExecutionPortStates;
        public string Session;
    }

    public class LiveExecutionAddListeners
    {
        public LiveListenerType ListenerType;
        public List<BlockKey> Blocklisteners;
        public List<GraphDescriptorKey> GraphListeners;
        public List<PortKey> PortListeners;
        public ListenerKey Listenerid;
    }

    public class LiveExecutionRemoveListeners
    {
        public LiveListenerType ListenerType;
        public List<BlockKey> Blocklisteners;
        public List<GraphDescriptorKey> GraphListeners;
        public List<PortKey> PortListeners;
        public ListenerKey Listenerid;
    }


    public enum EventMsgType
    {
        // TODO: add more message types
        Connection = 0,
        Notifications,
        AuthKey, // UserKey provided by client
        BlockAction,
        UserTrace,
        LiveExecutionCtrl,
        LiveExecutionBlockEvent,
        LiveExecutionPortEvent,
        ThingNameUpdate,
        // Sent to user to signal that a Thing was updated,
        ThingShared,
        ThingUnshared,
        SampleGraphsRemoved,
        PortEventMsg,
        IncidentReport,
        UserGraphException,
        ThingSharePending,
        /*
         * User related events.
         *  On user ban/eject/park/remove we need to logout user from frontend
         *  When user privilege and/or account level changes we need to update the user descriptor
         */
        LogoutUser,
        UpdateUser
    }


    [Flags]
    public enum UpdateInfo
    {
        None = 0x0,
        Name = 0x1,
        Config = 0x2,
        Tags = 0x4,
        State = 0x8,
        Icon = 0x10,
        All = 0x20
    }

    public class EmailRequest
    {
        public UserKey UserKey;
        public string Subject;
        public string Body;
    }

    #endregion

    public class GetThingKeyRsp : GenericRsp
    {
        public ThingKey ThingKey;
        public bool IsLoRaThing;
    }

    //------------------------------------------------------------------------------------------------------------------------
    public class CreateValueTriggerGraphEvent : WarlockApiMsg
    {
        public System.String Email;
        public Yodiwo.API.Warlock.PortRule Rule;
        public Yodiwo.API.Plegma.PortKey PortKey;
    }
    //------------------------------------------------------------------------------------------------------------------------
    public class CreateWatchdogGraphEvent : WarlockApiMsg
    {
        public System.String Email;
        public Yodiwo.API.Warlock.PortRule Rule;
        public Yodiwo.API.Plegma.PortKey PortKey;
        public System.TimeSpan Timer;
    }
    //------------------------------------------------------------------------------------------------------------------------
    public class CreateGraphRsp : GenericValueST<GraphDescriptorKey> { };

    public enum UserPenaltyAction
    {
        None,
        Ban,
        UnBan,
    };
}
