using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Yodiwo;
using Yodiwo.API.Plegma;
using Yodiwo.API.Warlock.Private;


namespace Yodiwo.API.Warlock
{
    //-----------------------------------------AUTO GENERATED CODE------------------------------------------------------------
    public class GetNodeDescriptorsReq : WarlockApiMsg
    {
	    public List<NodeKey> NodeKeys;
	    public Yodiwo.API.Plegma.eNodeType NodeType;
	    public System.String NodeName;
	    public System.Boolean parseAllThings;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class NodeCreateReq : WarlockApiMsg
    {
	    public System.String Name;
	    public Yodiwo.API.Plegma.eNodeType Type;
	    public Yodiwo.API.Warlock.eNodePermissions Permissions;
	    public Dictionary<String,String> AdditionalInfo;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class NodeDisableReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.NodeKey nodekey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class NodeEnableReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.NodeKey nodekey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class NodeUpdateReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.NodeKey Nodekey;
	    public System.String Name;
	    public Yodiwo.API.Warlock.eNodePermissions NodePermissions;
	    public Yodiwo.API.Warlock.NodeUpdateAction Action;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetThingTypesReq : WarlockApiMsg
    {
	    public System.Boolean SearchForYodiwoTypes;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetThingGroupReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.GroupKey GroupKey;
	    public System.Boolean FillThings;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class CreateNewGroup : WarlockApiMsg
    {
	    public System.String Name;
	    public System.String Type;
	    public System.String Icon;
	    public HashSet<String> Tags;
	    public List<ThingDescriptor> Things;
	    public System.Boolean AutoUpdate;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class UpdateGroup : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.GroupKey GroupKey;
	    public System.String Name;
	    public System.String Type;
	    public System.String Icon;
	    public HashSet<String> Tags;
	    public System.Boolean AutoUpdate;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class DeleteThingGroupReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.GroupKey Groupkey;
	    public Yodiwo.API.Plegma.ThingKey Thingkey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class UpdateThingReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.ThingKey ThingKey;
	    public List<ConfigParameter> Config;
	    public System.String Name;
	    public List<String> Tags;
	    public System.String IconURI;
	    public Yodiwo.API.Warlock.UpdateInfo updateinfo;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class MultipleThingActionReq : WarlockApiMsg
    {
	    public List<ThingKey> ThingKeys;
	    public Yodiwo.API.Warlock.ThingAction Action;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class ThingSetStateReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.PortEventMsg PortEvMsg;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class ScanReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.NodeKey NodeKey;
	    public System.String Id;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetThingKeyReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.NodeKey NodeKey;
	    public System.Boolean IsVirtual;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetPortReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.PortKey PortKey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class AddPortRule : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.PortKey PortKey;
	    public Yodiwo.API.Warlock.PortRule Rule;
	    public System.Boolean Clear;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetThingsReq : WarlockApiMsg
    {
	    public System.String key;
	    public List<ThingKey> Thingkeys;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class AddThingReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.Thing thing;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>Multiple share action requests.</summary>
    ///<para>The response to this message is <see cref="ShareThingsRsp"/></para>
    public class ShareThingsReq : WarlockApiMsg
    {
	    ///<summary>Array of <see cref</summary>
	    public Yodiwo.API.Warlock.ShareActionReq[] ShareActionReqs;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetUserDescriptorReq : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class SetUserSettings : WarlockApiMsg
    {
	    public System.String email;
	    public System.String TimeZoneid;
	    public System.String SharingOption;
	    public System.String FriendAddOption;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class CreateGmailAccountReq : WarlockApiMsg
    {
	    public System.String jsongmail;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetAllApiKeysReq : WarlockApiMsg
    {
	    public List<UserApiKey> userApiKeys;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class RemoveApiKeyReq : WarlockApiMsg
    {
	    public System.String apiKey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class EnableDisableApiKey : WarlockApiMsg
    {
	    public System.String apiKey;
	    public System.String action;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class CreateApiKeyReqForOAuth : WarlockApiMsg
    {
	    public System.String clientId;
	    public System.String scopes;
	    public System.String redirectUrl;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class AddFriendReq : WarlockApiMsg
    {
	    public System.String Key;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class DeleteFriendReq : WarlockApiMsg
    {
	    public System.String name;
	    public System.String key;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetFriendsReq : WarlockApiMsg
    {
	    public List<String> Emails;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class CreateUserApiKeyInfo : WarlockApiMsg
    {
	    public System.String name;
	    public Yodiwo.API.Warlock.eApiKeyType type;
	    public System.String key;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class LinkApiKeytoNode : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.NodeKey nk;
	    public Yodiwo.API.Plegma.UserApiKey userapikey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetUserTokensReq : WarlockApiMsg
    {
	    public System.String[] Emails;
	    public System.String[] Names;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class MyHackingReq : WarlockApiMsg
    {
	    public System.String AppToken;
	    public System.String AppId;
	    public System.String ThingKey;
	    public System.String NodeKey;
	    public Dictionary<String,String> Data;
	    public Dictionary<String,Boolean> IsDataString;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class AddNewLinkedUserReq : WarlockApiMsg
    {
	    public Yodiwo.API.Warlock.LinkedUserInfo LinkedUserInfo;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class RemoveLinkedUserReq : WarlockApiMsg
    {
	    public System.String LinkedUserEmail;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class UpdateLinkedUserReq : WarlockApiMsg
    {
	    public Yodiwo.API.Warlock.LinkedUserInfo linkedUserInfo;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetLinkedUserReq : WarlockApiMsg
    {
	    public System.String linkedUserEmail;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class RereadQuotaDataTablesReq : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>get quota for user; if target user different than asking user, it requires QuotaAdmin privilege level and upwards</summary>
    ///<para>The response to this message is <see cref="GetUserQuotaRsp"/></para>
    public class GetUserQuotaReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.UserKey targetUkey;
	    public System.String targetKey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>set new quota limit; requires at least QuotaAdmin privileges</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class SetQuotaLimit : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.UserKey targetUkey;
	    public System.String targetKey;
	    public Yodiwo.API.Warlock.eQuotaType type;
	    public System.Int64 limit;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>force set value for current quota period; quota type must be resettable; requires at least QuotaAdmin privileges</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class SetQuotaCurrentValue : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.UserKey targetUkey;
	    public System.String targetKey;
	    public Yodiwo.API.Warlock.eQuotaType type;
	    public System.Int64 value;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>reset quota counting period; quota type must be resettable; requires at least QuotaAdmin privileges</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class ResetQuotaPeriod : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.UserKey targetUkey;
	    public System.String targetKey;
	    public Yodiwo.API.Warlock.eQuotaType type;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>set new quota period timespan; quota type must be resettable; requires at least QuotaAdmin privileges</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class SetQuotaPeriodTimespan : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.UserKey targetUkey;
	    public System.String targetKey;
	    public Yodiwo.API.Warlock.eQuotaType type;
	    public System.TimeSpan period;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>set quota flags that are changeable; values of non-changeable flags are ignored; requires at least QuotaAdmin privileges</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class SetQuotaFlags : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.UserKey targetUkey;
	    public System.String targetKey;
	    public Yodiwo.API.Warlock.eQuotaType type;
	    public Yodiwo.API.Warlock.eQuotaFlags flagsToSet;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>clear quota flags that are changeable; values of non-changeable flags are ignored; requires at least QuotaAdmin privileges</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class ClearQuotaFlags : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.UserKey targetUkey;
	    public System.String targetNkey;
	    public Yodiwo.API.Warlock.eQuotaType type;
	    public Yodiwo.API.Warlock.eQuotaFlags flagsToClear;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>set new Percentage value that should trigger an event if crossed</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class SetAlertPercentage : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.UserKey targetUkey;
	    public System.String targetKey;
	    public Yodiwo.API.Warlock.eQuotaType type;
	    public System.Int32 newPercentage;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>set new Percentage value for all quota that should trigger an event if crossed</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class SetAlertPercentageForAllQuota : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.UserKey targetUkey;
	    public System.Int32 newPercentage;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>set or reset DefyQuotaLimits flag; If targetNkey is null, setting applies to full target User; If it's set and valid, the setting will apply to specific node; requires at least QuotaAdmin privileges</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class SetDefyQuotaFlag : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.UserKey targetUkey;
	    public System.String targetNKey;
	    public System.Boolean newValue;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>get all available users, either online or offline; requires ReadAdmin privilege level</summary>
    ///<para>The response to this message is <see cref="GetAllUserDescriptorsRsp"/></para>
    public class GetAllUserDescriptorsReq : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>change user's privilege level; requires WriteAdmin privilege level</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class ChangePrivilegeLevel : WarlockApiMsg
    {
	    public System.String email;
	    public Yodiwo.API.Warlock.ePrivilegeLevel privilege;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>change user's account level; requires WriteAdmin privilege level</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class ChangeAccountLevel : WarlockApiMsg
    {
	    public System.String email;
	    public System.Int32 accountLevel;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>remove a user from memory (requires WriteAdmin privileges)</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class ParkUserReq : WarlockApiMsg
    {
	    ///<summary> user's email</summary>
	    public System.String email;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>delete a user (requires WriteAdmin privileges)</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class DeleteUserReq : WarlockApiMsg
    {
	    ///<summary>user's email</summary>
	    public System.String email;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>eject a user (requires WriteAdmin privileges)</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class EjectUserReq : WarlockApiMsg
    {
	    ///<summary>user's email</summary>
	    public System.String email;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>ban/unban a user (requires WriteAdmin privileges)</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class BanUnBanUserReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.UserKey targetUserKey;
	    public Yodiwo.API.Warlock.UserPenaltyAction penaltyAction;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>undeploy a graph that belong to another user (requires WriteAdmin privileges)</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class UndeployExUserGraphReq : WarlockApiMsg
    {
	    ///<summary>user's graphdescriptor key</summary>
	    public Yodiwo.API.Plegma.GraphDescriptorKey graphDescriptorKey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>get id from a user (requires ReadAdmin privileges)</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class GetUserIdReq : WarlockApiMsg
    {
	    ///<summary>user's email</summary>
	    public System.String email;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>change global smtp password (requires special privileges)</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class ChangeSmtpPasswordRequest : WarlockApiMsg
    {
	    public System.String pwd;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>clear all services from a user (requires WriteAdmin privileges)</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class ClearRestServicesReq : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class ChangeDebugLevelReq : WarlockApiMsg
    {
	    public Yodiwo.eTraceType level;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>recover an already paired node</summary>
    public class NodePairingRecoverNodesReq : WarlockApiMsg
    {
	    ///<summary> token which is linked to a pairing context</summary>
	    public System.String token2;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>complete pairing procedure</summary>
    public class NodePairingCompleteReq : WarlockApiMsg
    {
	    ///<summary>token which is linked to a pairing context</summary>
	    public System.String token2;
	    public System.String uuid;
	    public System.String nodekey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>get the number of saved and deployed graphs</summary>
    ///<para>The response to this message is <see cref="DashboardGraphsResponse"/></para>
    public class EnumerateSaveandDeployedGraphsReq : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>get user's social accounts descriptors(i.e facebook, hangouts, gmail</summary>
    ///<para>The response to this message is <see cref="SocialAccountsRsp"/></para>
    public class GetSocialAccountsReq : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>used for unpairing a node</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class UnpairNodeReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.NodeKey Nodekey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>remove a social account from user's profile</summary>
    ///<para>The response to this message is <see cref="GenericRsp"/></para>
    public class RemoveSocialAccountReq : WarlockApiMsg
    {
	    public System.String Type;
	    public System.String Data;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>get toolbox modelview</summary>
    public class GetToolboxModelViewReq : WarlockApiMsg
    {
	    public System.Boolean useCache;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>get block descriptor for a blocktype</summary>
    public class GetBlockDescriptorforBlockType : WarlockApiMsg
    {
	    ///<summary>specific block type</summary>
	    public System.String type;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetBlockDescriptorforThingReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.ThingKey tkey;
	    public System.String type;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetBlockDescriptorforGroupReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.GroupKey gkey;
	    public System.String type;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class LoadGraphByFriendlyName : WarlockApiMsg
    {
	    public System.String Path;
	    public System.String Name;
	}
	//------------------------------------------------------------------------------------------------------------------------
    ///<summary>get all uploaded images</summary>
    public class GetAllImagesReq : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetImageReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.BinaryResourceDescriptorKey brdkey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class AddResourceDescriptorRequest : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.GraphDescriptorKey gKey;
	    public Yodiwo.API.Plegma.BinaryResourceDescriptorKey brdKey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class DeleteDescriptorRequest : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.GraphDescriptorKey gKey;
	    public Yodiwo.API.Plegma.BinaryResourceDescriptorKey brdKey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetAllPortKeys : WarlockApiMsg
    {
	    public List<String> AllThingKeysStr;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetAllPorts : WarlockApiMsg
    {
	    public List<String> AllThingKeysStr;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class PortConfigurationReq : WarlockApiMsg
    {
	    public System.String portKey;
	    public System.String stateColor;
	    public System.Int32 stateSize;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class BRDUploadReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.BinaryResourceDescriptor Binaryresourcedescriptor;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetBinaryResourceDesciptorsRequest : WarlockApiMsg
    {
	    public BinaryResourceDescriptorKey BinaryresourcedescriptorKey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class CreateSnapshotReq : WarlockApiMsg
    {
	    public List<String> PortKeysStr;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class LoadGraphsReq : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class LoadFilesReq : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class DeleteUploadedFileReq : WarlockApiMsg
    {
	    public Yodiwo.API.Plegma.BinaryResourceDescriptorKey BrdKey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class RequestExternalGraphAction : WarlockApiMsg
    {
	    public Yodiwo.API.Warlock.BlockActionReq blockMessage;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetAllAvailableRestServicesReq : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GraphActionsReq : WarlockApiMsg
    {
	    public Yodiwo.API.Warlock.GraphActionReq[] Actions;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class ShareGraphActionReq : WarlockApiMsg
    {
	    public Yodiwo.API.Warlock.ShareGraphReq request;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class SharedGraphConfigReq : WarlockApiMsg
    {
	    public Yodiwo.API.Warlock.SharedGraphConfReq request;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class ConfigureUserTimezoneReq : WarlockApiMsg
    {
	    public System.String email;
	    public System.String TimezoneId;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class ConfigureUserMetadataReq : WarlockApiMsg
    {
	    public System.String email;
	    public System.String name;
	    public System.String firstname;
	    public System.String lastname;
	    public System.String uname;
	    public System.String avataruri;
	    public System.String locale;
	    public System.DateTime dt;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class NotificationMarkAsRead : WarlockApiMsg
    {
	    public System.String Nodekey;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class NotificationMarkAllAsRead : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetAllNotificationsReq : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class AddWizardRestServiceReq : WarlockApiMsg
    {
	    public Yodiwo.API.Warlock.Private.RestServiceWizardDescriptor rswd;
	    public System.Boolean notbase;
	    public System.String serviceName;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class AddWizardConfiguredRestServiceReq : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GlobalConfiguredServiceReq : WarlockApiMsg
    {
	    public Yodiwo.API.Warlock.Private.ConfiguredRestServiceWizardDescriptor newCServiceInfo;
	    public System.String service;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class AddRestServiceReq : WarlockApiMsg
    {
	    public System.String jsonrestservice;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetAllRestServicesReq : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetAllSRestDescriptors : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class TryGetRestServiceDescriptor : WarlockApiMsg
    {
	    public System.String service;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class FindUserRestServiceDescriptor : WarlockApiMsg
    {
	    public System.String service;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class RestAuth : WarlockApiMsg
    {
	    public Yodiwo.API.Warlock.Private.ConfiguredRestServiceWizardDescriptor CServiceInfo;
	    public System.String restService;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class CreateFirstAuthUrl : WarlockApiMsg
    {
	    public Yodiwo.API.Warlock.Private.ConfiguredRestServiceWizardDescriptor CServiceInfo;
	    public System.String restService;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class AddConfiguredRestService : WarlockApiMsg
    {
	    public Yodiwo.API.Warlock.Private.ConfiguredRestServiceWizardDescriptor CServiceInfo;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class AddSocialAccount : WarlockApiMsg
    {
	    public System.Object socialdesc;
	    public System.Type desctype;
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class AssignSipAccount : WarlockApiMsg
    {
	}
	//------------------------------------------------------------------------------------------------------------------------
    public class GetSharedThingsReq : WarlockApiMsg
    {
	    public List<ThingKey> ThingKeys;
	}
	//------------------------------------------------------------------------------------------------------------------------
}
