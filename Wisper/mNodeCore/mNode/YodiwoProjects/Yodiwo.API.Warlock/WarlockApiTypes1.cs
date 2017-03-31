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
//-----------------------------------------AUTO GENERATED CODE--------------------------------------------------------------
	public static partial class WarlockAPI
    {
        /// <summary>api current version</summary>
        public const int APIVersion = 1;

        /// <summary>internal api name</summary>
        public const string ApiGroupName = "WarlockAPI";

		
        /// <summary>
        /// List of all possible API messages that are exchanged between Nodes and the Yodiwo Cloud Service
        /// </summary>
        public static Type[] ApiMessages =
        {
				typeof(AssignSipAccount),
			typeof(GetSharedThingsReq),
			typeof(GetNodeDescriptorsReq),
			typeof(NodeCreateReq),
			typeof(NodeDisableReq),
			typeof(NodeEnableReq),
			typeof(NodeUpdateReq),
			typeof(GetThingTypesReq),
			typeof(GetThingGroupReq),
			typeof(CreateNewGroup),
			typeof(UpdateGroup),
			typeof(DeleteThingGroupReq),
			typeof(UpdateThingReq),
			typeof(MultipleThingActionReq),
			typeof(ThingSetStateReq),
			typeof(ScanReq),
			typeof(GetThingKeyReq),
			typeof(GetPortReq),
			typeof(AddPortRule),
			typeof(GetThingsReq),
			typeof(AddThingReq),
			typeof(ShareThingsReq),
			typeof(GetUserDescriptorReq),
			typeof(SetUserSettings),
			typeof(CreateGmailAccountReq),
			typeof(GetAllApiKeysReq),
			typeof(RemoveApiKeyReq),
			typeof(EnableDisableApiKey),
			typeof(CreateApiKeyReqForOAuth),
			typeof(AddFriendReq),
			typeof(DeleteFriendReq),
			typeof(GetFriendsReq),
			typeof(CreateUserApiKeyInfo),
			typeof(LinkApiKeytoNode),
			typeof(GetUserTokensReq),
			typeof(AddNewLinkedUserReq),
			typeof(RemoveLinkedUserReq),
			typeof(UpdateLinkedUserReq),
			typeof(GetLinkedUsersReq),
			typeof(MyHackingReq),
			typeof(GetDatabasesInfoReq),
			typeof(GetDatabasesUsersInfoReq),
			typeof(GetDatabaseInstancesReq),
			typeof(GetDatabaseUsersInstancesReq),
			typeof(DatabaseActionReq),
			typeof(DatabaseMultiActionReq),
			typeof(RereadQuotaDataTablesReq),
			typeof(GetUserQuotaReq),
			typeof(SetQuotaLimit),
			typeof(SetQuotaCurrentValue),
			typeof(ResetQuotaPeriod),
			typeof(SetQuotaPeriodTimespan),
			typeof(SetQuotaFlags),
			typeof(ClearQuotaFlags),
			typeof(SetAlertPercentage),
			typeof(SetAlertPercentageForAllQuota),
			typeof(SetDefyQuotaFlag),
			typeof(GetAllUserDescriptorsReq),
			typeof(ChangePrivilegeLevel),
			typeof(ChangeAccountLevel),
			typeof(ParkUserReq),
			typeof(DeleteUserReq),
			typeof(EjectUserReq),
			typeof(BanUnBanUserReq),
			typeof(UndeployExUserGraphReq),
			typeof(GetUserIdReq),
			typeof(ChangeSmtpPasswordRequest),
			typeof(ClearRestServicesReq),
			typeof(ChangeDebugLevelReq),
			typeof(NodePairingRecoverNodesReq),
			typeof(NodePairingCompleteReq),
			typeof(EnumerateSaveandDeployedGraphsReq),
			typeof(GetSocialAccountsReq),
			typeof(UnpairNodeReq),
			typeof(RemoveSocialAccountReq),
			typeof(GetToolboxModelViewReq),
			typeof(GetBlockDescriptorforBlockType),
			typeof(GetBlockDescriptorforThingReq),
			typeof(GetBlockDescriptorforGroupReq),
			typeof(LoadGraphByFriendlyName),
			typeof(GetAllImagesReq),
			typeof(GetImageReq),
			typeof(AddResourceDescriptorRequest),
			typeof(DeleteDescriptorRequest),
			typeof(GetAllPortKeys),
			typeof(GetAllPorts),
			typeof(PortConfigurationReq),
			typeof(CreateMatchingVirtualThingReq),
			typeof(BRDUploadReq),
			typeof(GetBinaryResourceDesciptorsRequest),
			typeof(CreateSnapshotReq),
			typeof(LoadGraphsReq),
			typeof(LoadFilesReq),
			typeof(DeleteUploadedFileReq),
			typeof(RequestExternalGraphAction),
			typeof(GetAllAvailableRestServicesReq),
			typeof(GraphActionsReq),
			typeof(ShareGraphActionReq),
			typeof(SharedGraphConfigReq),
			typeof(ConfigureUserTimezoneReq),
			typeof(ConfigureUserMetadataReq),
			typeof(NotificationMarkAsRead),
			typeof(NotificationMarkAllAsRead),
			typeof(GetAllNotificationsReq),
			typeof(AddWizardRestServiceReq),
			typeof(AddWizardConfiguredRestServiceReq),
			typeof(GlobalConfiguredServiceReq),
			typeof(AddRestServiceReq),
			typeof(GetAllRestServicesReq),
			typeof(GetAllSRestDescriptors),
			typeof(TryGetRestServiceDescriptor),
			typeof(FindUserRestServiceDescriptor),
			typeof(RestAuth),
			typeof(CreateFirstAuthUrl),
			typeof(AddConfiguredRestService),
			typeof(AddSocialAccount),
						typeof(GenericValue),
			typeof(GetSharedThingsRsp),
			typeof(NodeDescriptorsRsp),
			typeof(NodeCreateRsp),
			typeof(GenericRsp),
			typeof(GetThingTypesRsp),
			typeof(GroupsRsp),
			typeof(CreateGroupRsp),
			typeof(MultipleThingActionRsp),
			typeof(ScanRsp),
			typeof(GetThingKeyRsp),
			typeof(GetPortResp),
			typeof(GetThingsRsp),
			typeof(ShareThingsRsp),
			typeof(GetUserDescriptorRsp),
			typeof(SetUserSettingsRsp),
			typeof(ApiKeyRsp),
			typeof(OAuthGetTokenResponse),
			typeof(GetFriendsRsp),
			typeof(CreateApiKeyRsp),
			typeof(GetUserTokensRsp),
			typeof(GetLinkedUserDescriptors),
			typeof(MyHackingRsp),
			typeof(GetDatabasesInfoRsp),
			typeof(GetDatabasesUsersInfoRsp),
			typeof(DatabaseMultiActionRsp),
			typeof(GetUserQuotaRsp),
			typeof(GetAllUserDescriptorsRsp),
			typeof(NodePairingRecoverRsp),
			typeof(NodePairingCompetionRsp),
			typeof(DashboardGraphsResponse),
			typeof(SocialAccountsRsp),
			typeof(GetToolboxModelViewRsp),
			typeof(GetBlockDescriptorRsp),
			typeof(BookmarkableLoadRoute),
			typeof(BinaryResourceDescRsp),
			typeof(GetAllPortKeysRsp),
			typeof(GetAllPortsRsp),
			typeof(GrafanaSnapshotCreateResponse),
			typeof(LoadGraphsResp),
			typeof(LoadFilesResp),
			typeof(MultiGraphActionRsp),
			typeof(SharedGraphConfRsp),
			typeof(GetUpdatedUserDescRsp),
			typeof(GetNotificationsRsp),
			typeof(GlobalConfiguredServiceResp),
			typeof(RestServiceRsp),
			typeof(SocialAccountRsp),
			};


			//Literal API names
			 public static string s_AssignSipAccount = nameof(AssignSipAccount).ToLower();
			 public static string s_GetSharedThingsReq = nameof(GetSharedThingsReq).ToLower();
			 public static string s_GetNodeDescriptorsReq = nameof(GetNodeDescriptorsReq).ToLower();
			 public static string s_NodeCreateReq = nameof(NodeCreateReq).ToLower();
			 public static string s_NodeDisableReq = nameof(NodeDisableReq).ToLower();
			 public static string s_NodeEnableReq = nameof(NodeEnableReq).ToLower();
			 public static string s_NodeUpdateReq = nameof(NodeUpdateReq).ToLower();
			 public static string s_GetThingTypesReq = nameof(GetThingTypesReq).ToLower();
			 public static string s_GetThingGroupReq = nameof(GetThingGroupReq).ToLower();
			 public static string s_CreateNewGroup = nameof(CreateNewGroup).ToLower();
			 public static string s_UpdateGroup = nameof(UpdateGroup).ToLower();
			 public static string s_DeleteThingGroupReq = nameof(DeleteThingGroupReq).ToLower();
			 public static string s_UpdateThingReq = nameof(UpdateThingReq).ToLower();
			 public static string s_MultipleThingActionReq = nameof(MultipleThingActionReq).ToLower();
			 public static string s_ThingSetStateReq = nameof(ThingSetStateReq).ToLower();
			 public static string s_ScanReq = nameof(ScanReq).ToLower();
			 public static string s_GetThingKeyReq = nameof(GetThingKeyReq).ToLower();
			 public static string s_GetPortReq = nameof(GetPortReq).ToLower();
			 public static string s_AddPortRule = nameof(AddPortRule).ToLower();
			 public static string s_GetThingsReq = nameof(GetThingsReq).ToLower();
			 public static string s_AddThingReq = nameof(AddThingReq).ToLower();
			 public static string s_ShareThingsReq = nameof(ShareThingsReq).ToLower();
			 public static string s_GetUserDescriptorReq = nameof(GetUserDescriptorReq).ToLower();
			 public static string s_SetUserSettings = nameof(SetUserSettings).ToLower();
			 public static string s_CreateGmailAccountReq = nameof(CreateGmailAccountReq).ToLower();
			 public static string s_GetAllApiKeysReq = nameof(GetAllApiKeysReq).ToLower();
			 public static string s_RemoveApiKeyReq = nameof(RemoveApiKeyReq).ToLower();
			 public static string s_EnableDisableApiKey = nameof(EnableDisableApiKey).ToLower();
			 public static string s_CreateApiKeyReqForOAuth = nameof(CreateApiKeyReqForOAuth).ToLower();
			 public static string s_AddFriendReq = nameof(AddFriendReq).ToLower();
			 public static string s_DeleteFriendReq = nameof(DeleteFriendReq).ToLower();
			 public static string s_GetFriendsReq = nameof(GetFriendsReq).ToLower();
			 public static string s_CreateUserApiKeyInfo = nameof(CreateUserApiKeyInfo).ToLower();
			 public static string s_LinkApiKeytoNode = nameof(LinkApiKeytoNode).ToLower();
			 public static string s_GetUserTokensReq = nameof(GetUserTokensReq).ToLower();
			 public static string s_AddNewLinkedUserReq = nameof(AddNewLinkedUserReq).ToLower();
			 public static string s_RemoveLinkedUserReq = nameof(RemoveLinkedUserReq).ToLower();
			 public static string s_UpdateLinkedUserReq = nameof(UpdateLinkedUserReq).ToLower();
			 public static string s_GetLinkedUsersReq = nameof(GetLinkedUsersReq).ToLower();
			 public static string s_MyHackingReq = nameof(MyHackingReq).ToLower();
			 public static string s_GetDatabasesInfoReq = nameof(GetDatabasesInfoReq).ToLower();
			 public static string s_GetDatabasesUsersInfoReq = nameof(GetDatabasesUsersInfoReq).ToLower();
			 public static string s_GetDatabaseInstancesReq = nameof(GetDatabaseInstancesReq).ToLower();
			 public static string s_GetDatabaseUsersInstancesReq = nameof(GetDatabaseUsersInstancesReq).ToLower();
			 public static string s_DatabaseActionReq = nameof(DatabaseActionReq).ToLower();
			 public static string s_DatabaseMultiActionReq = nameof(DatabaseMultiActionReq).ToLower();
			 public static string s_RereadQuotaDataTablesReq = nameof(RereadQuotaDataTablesReq).ToLower();
			 public static string s_GetUserQuotaReq = nameof(GetUserQuotaReq).ToLower();
			 public static string s_SetQuotaLimit = nameof(SetQuotaLimit).ToLower();
			 public static string s_SetQuotaCurrentValue = nameof(SetQuotaCurrentValue).ToLower();
			 public static string s_ResetQuotaPeriod = nameof(ResetQuotaPeriod).ToLower();
			 public static string s_SetQuotaPeriodTimespan = nameof(SetQuotaPeriodTimespan).ToLower();
			 public static string s_SetQuotaFlags = nameof(SetQuotaFlags).ToLower();
			 public static string s_ClearQuotaFlags = nameof(ClearQuotaFlags).ToLower();
			 public static string s_SetAlertPercentage = nameof(SetAlertPercentage).ToLower();
			 public static string s_SetAlertPercentageForAllQuota = nameof(SetAlertPercentageForAllQuota).ToLower();
			 public static string s_SetDefyQuotaFlag = nameof(SetDefyQuotaFlag).ToLower();
			 public static string s_GetAllUserDescriptorsReq = nameof(GetAllUserDescriptorsReq).ToLower();
			 public static string s_ChangePrivilegeLevel = nameof(ChangePrivilegeLevel).ToLower();
			 public static string s_ChangeAccountLevel = nameof(ChangeAccountLevel).ToLower();
			 public static string s_ParkUserReq = nameof(ParkUserReq).ToLower();
			 public static string s_DeleteUserReq = nameof(DeleteUserReq).ToLower();
			 public static string s_EjectUserReq = nameof(EjectUserReq).ToLower();
			 public static string s_BanUnBanUserReq = nameof(BanUnBanUserReq).ToLower();
			 public static string s_UndeployExUserGraphReq = nameof(UndeployExUserGraphReq).ToLower();
			 public static string s_GetUserIdReq = nameof(GetUserIdReq).ToLower();
			 public static string s_ChangeSmtpPasswordRequest = nameof(ChangeSmtpPasswordRequest).ToLower();
			 public static string s_ClearRestServicesReq = nameof(ClearRestServicesReq).ToLower();
			 public static string s_ChangeDebugLevelReq = nameof(ChangeDebugLevelReq).ToLower();
			 public static string s_NodePairingRecoverNodesReq = nameof(NodePairingRecoverNodesReq).ToLower();
			 public static string s_NodePairingCompleteReq = nameof(NodePairingCompleteReq).ToLower();
			 public static string s_EnumerateSaveandDeployedGraphsReq = nameof(EnumerateSaveandDeployedGraphsReq).ToLower();
			 public static string s_GetSocialAccountsReq = nameof(GetSocialAccountsReq).ToLower();
			 public static string s_UnpairNodeReq = nameof(UnpairNodeReq).ToLower();
			 public static string s_RemoveSocialAccountReq = nameof(RemoveSocialAccountReq).ToLower();
			 public static string s_GetToolboxModelViewReq = nameof(GetToolboxModelViewReq).ToLower();
			 public static string s_GetBlockDescriptorforBlockType = nameof(GetBlockDescriptorforBlockType).ToLower();
			 public static string s_GetBlockDescriptorforThingReq = nameof(GetBlockDescriptorforThingReq).ToLower();
			 public static string s_GetBlockDescriptorforGroupReq = nameof(GetBlockDescriptorforGroupReq).ToLower();
			 public static string s_LoadGraphByFriendlyName = nameof(LoadGraphByFriendlyName).ToLower();
			 public static string s_GetAllImagesReq = nameof(GetAllImagesReq).ToLower();
			 public static string s_GetImageReq = nameof(GetImageReq).ToLower();
			 public static string s_AddResourceDescriptorRequest = nameof(AddResourceDescriptorRequest).ToLower();
			 public static string s_DeleteDescriptorRequest = nameof(DeleteDescriptorRequest).ToLower();
			 public static string s_GetAllPortKeys = nameof(GetAllPortKeys).ToLower();
			 public static string s_GetAllPorts = nameof(GetAllPorts).ToLower();
			 public static string s_PortConfigurationReq = nameof(PortConfigurationReq).ToLower();
			 public static string s_CreateMatchingVirtualThingReq = nameof(CreateMatchingVirtualThingReq).ToLower();
			 public static string s_BRDUploadReq = nameof(BRDUploadReq).ToLower();
			 public static string s_GetBinaryResourceDesciptorsRequest = nameof(GetBinaryResourceDesciptorsRequest).ToLower();
			 public static string s_CreateSnapshotReq = nameof(CreateSnapshotReq).ToLower();
			 public static string s_LoadGraphsReq = nameof(LoadGraphsReq).ToLower();
			 public static string s_LoadFilesReq = nameof(LoadFilesReq).ToLower();
			 public static string s_DeleteUploadedFileReq = nameof(DeleteUploadedFileReq).ToLower();
			 public static string s_RequestExternalGraphAction = nameof(RequestExternalGraphAction).ToLower();
			 public static string s_GetAllAvailableRestServicesReq = nameof(GetAllAvailableRestServicesReq).ToLower();
			 public static string s_GraphActionsReq = nameof(GraphActionsReq).ToLower();
			 public static string s_ShareGraphActionReq = nameof(ShareGraphActionReq).ToLower();
			 public static string s_SharedGraphConfigReq = nameof(SharedGraphConfigReq).ToLower();
			 public static string s_ConfigureUserTimezoneReq = nameof(ConfigureUserTimezoneReq).ToLower();
			 public static string s_ConfigureUserMetadataReq = nameof(ConfigureUserMetadataReq).ToLower();
			 public static string s_NotificationMarkAsRead = nameof(NotificationMarkAsRead).ToLower();
			 public static string s_NotificationMarkAllAsRead = nameof(NotificationMarkAllAsRead).ToLower();
			 public static string s_GetAllNotificationsReq = nameof(GetAllNotificationsReq).ToLower();
			 public static string s_AddWizardRestServiceReq = nameof(AddWizardRestServiceReq).ToLower();
			 public static string s_AddWizardConfiguredRestServiceReq = nameof(AddWizardConfiguredRestServiceReq).ToLower();
			 public static string s_GlobalConfiguredServiceReq = nameof(GlobalConfiguredServiceReq).ToLower();
			 public static string s_AddRestServiceReq = nameof(AddRestServiceReq).ToLower();
			 public static string s_GetAllRestServicesReq = nameof(GetAllRestServicesReq).ToLower();
			 public static string s_GetAllSRestDescriptors = nameof(GetAllSRestDescriptors).ToLower();
			 public static string s_TryGetRestServiceDescriptor = nameof(TryGetRestServiceDescriptor).ToLower();
			 public static string s_FindUserRestServiceDescriptor = nameof(FindUserRestServiceDescriptor).ToLower();
			 public static string s_RestAuth = nameof(RestAuth).ToLower();
			 public static string s_CreateFirstAuthUrl = nameof(CreateFirstAuthUrl).ToLower();
			 public static string s_AddConfiguredRestService = nameof(AddConfiguredRestService).ToLower();
			 public static string s_AddSocialAccount = nameof(AddSocialAccount).ToLower();
						public static string s_GenericValue = nameof(GenericValue).ToLower();
			public static string s_GetSharedThingsRsp = nameof(GetSharedThingsRsp).ToLower();
			public static string s_NodeDescriptorsRsp = nameof(NodeDescriptorsRsp).ToLower();
			public static string s_NodeCreateRsp = nameof(NodeCreateRsp).ToLower();
			public static string s_GenericRsp = nameof(GenericRsp).ToLower();
			public static string s_GetThingTypesRsp = nameof(GetThingTypesRsp).ToLower();
			public static string s_GroupsRsp = nameof(GroupsRsp).ToLower();
			public static string s_CreateGroupRsp = nameof(CreateGroupRsp).ToLower();
			public static string s_MultipleThingActionRsp = nameof(MultipleThingActionRsp).ToLower();
			public static string s_ScanRsp = nameof(ScanRsp).ToLower();
			public static string s_GetThingKeyRsp = nameof(GetThingKeyRsp).ToLower();
			public static string s_GetPortResp = nameof(GetPortResp).ToLower();
			public static string s_GetThingsRsp = nameof(GetThingsRsp).ToLower();
			public static string s_ShareThingsRsp = nameof(ShareThingsRsp).ToLower();
			public static string s_GetUserDescriptorRsp = nameof(GetUserDescriptorRsp).ToLower();
			public static string s_SetUserSettingsRsp = nameof(SetUserSettingsRsp).ToLower();
			public static string s_ApiKeyRsp = nameof(ApiKeyRsp).ToLower();
			public static string s_OAuthGetTokenResponse = nameof(OAuthGetTokenResponse).ToLower();
			public static string s_GetFriendsRsp = nameof(GetFriendsRsp).ToLower();
			public static string s_CreateApiKeyRsp = nameof(CreateApiKeyRsp).ToLower();
			public static string s_GetUserTokensRsp = nameof(GetUserTokensRsp).ToLower();
			public static string s_GetLinkedUserDescriptors = nameof(GetLinkedUserDescriptors).ToLower();
			public static string s_MyHackingRsp = nameof(MyHackingRsp).ToLower();
			public static string s_GetDatabasesInfoRsp = nameof(GetDatabasesInfoRsp).ToLower();
			public static string s_GetDatabasesUsersInfoRsp = nameof(GetDatabasesUsersInfoRsp).ToLower();
			public static string s_DatabaseMultiActionRsp = nameof(DatabaseMultiActionRsp).ToLower();
			public static string s_GetUserQuotaRsp = nameof(GetUserQuotaRsp).ToLower();
			public static string s_GetAllUserDescriptorsRsp = nameof(GetAllUserDescriptorsRsp).ToLower();
			public static string s_NodePairingRecoverRsp = nameof(NodePairingRecoverRsp).ToLower();
			public static string s_NodePairingCompetionRsp = nameof(NodePairingCompetionRsp).ToLower();
			public static string s_DashboardGraphsResponse = nameof(DashboardGraphsResponse).ToLower();
			public static string s_SocialAccountsRsp = nameof(SocialAccountsRsp).ToLower();
			public static string s_GetToolboxModelViewRsp = nameof(GetToolboxModelViewRsp).ToLower();
			public static string s_GetBlockDescriptorRsp = nameof(GetBlockDescriptorRsp).ToLower();
			public static string s_BookmarkableLoadRoute = nameof(BookmarkableLoadRoute).ToLower();
			public static string s_BinaryResourceDescRsp = nameof(BinaryResourceDescRsp).ToLower();
			public static string s_GetAllPortKeysRsp = nameof(GetAllPortKeysRsp).ToLower();
			public static string s_GetAllPortsRsp = nameof(GetAllPortsRsp).ToLower();
			public static string s_GrafanaSnapshotCreateResponse = nameof(GrafanaSnapshotCreateResponse).ToLower();
			public static string s_LoadGraphsResp = nameof(LoadGraphsResp).ToLower();
			public static string s_LoadFilesResp = nameof(LoadFilesResp).ToLower();
			public static string s_MultiGraphActionRsp = nameof(MultiGraphActionRsp).ToLower();
			public static string s_SharedGraphConfRsp = nameof(SharedGraphConfRsp).ToLower();
			public static string s_GetUpdatedUserDescRsp = nameof(GetUpdatedUserDescRsp).ToLower();
			public static string s_GetNotificationsRsp = nameof(GetNotificationsRsp).ToLower();
			public static string s_GlobalConfiguredServiceResp = nameof(GlobalConfiguredServiceResp).ToLower();
			public static string s_RestServiceRsp = nameof(RestServiceRsp).ToLower();
			public static string s_SocialAccountRsp = nameof(SocialAccountRsp).ToLower();
	

		public static Dictionary<Type, String> ApiMsgNames = new Dictionary<Type, string>()
        {
			
				{typeof(AssignSipAccount),s_AssignSipAccount },
			{typeof(GetSharedThingsReq),s_GetSharedThingsReq },
			{typeof(GetNodeDescriptorsReq),s_GetNodeDescriptorsReq },
			{typeof(NodeCreateReq),s_NodeCreateReq },
			{typeof(NodeDisableReq),s_NodeDisableReq },
			{typeof(NodeEnableReq),s_NodeEnableReq },
			{typeof(NodeUpdateReq),s_NodeUpdateReq },
			{typeof(GetThingTypesReq),s_GetThingTypesReq },
			{typeof(GetThingGroupReq),s_GetThingGroupReq },
			{typeof(CreateNewGroup),s_CreateNewGroup },
			{typeof(UpdateGroup),s_UpdateGroup },
			{typeof(DeleteThingGroupReq),s_DeleteThingGroupReq },
			{typeof(UpdateThingReq),s_UpdateThingReq },
			{typeof(MultipleThingActionReq),s_MultipleThingActionReq },
			{typeof(ThingSetStateReq),s_ThingSetStateReq },
			{typeof(ScanReq),s_ScanReq },
			{typeof(GetThingKeyReq),s_GetThingKeyReq },
			{typeof(GetPortReq),s_GetPortReq },
			{typeof(AddPortRule),s_AddPortRule },
			{typeof(GetThingsReq),s_GetThingsReq },
			{typeof(AddThingReq),s_AddThingReq },
			{typeof(ShareThingsReq),s_ShareThingsReq },
			{typeof(GetUserDescriptorReq),s_GetUserDescriptorReq },
			{typeof(SetUserSettings),s_SetUserSettings },
			{typeof(CreateGmailAccountReq),s_CreateGmailAccountReq },
			{typeof(GetAllApiKeysReq),s_GetAllApiKeysReq },
			{typeof(RemoveApiKeyReq),s_RemoveApiKeyReq },
			{typeof(EnableDisableApiKey),s_EnableDisableApiKey },
			{typeof(CreateApiKeyReqForOAuth),s_CreateApiKeyReqForOAuth },
			{typeof(AddFriendReq),s_AddFriendReq },
			{typeof(DeleteFriendReq),s_DeleteFriendReq },
			{typeof(GetFriendsReq),s_GetFriendsReq },
			{typeof(CreateUserApiKeyInfo),s_CreateUserApiKeyInfo },
			{typeof(LinkApiKeytoNode),s_LinkApiKeytoNode },
			{typeof(GetUserTokensReq),s_GetUserTokensReq },
			{typeof(AddNewLinkedUserReq),s_AddNewLinkedUserReq },
			{typeof(RemoveLinkedUserReq),s_RemoveLinkedUserReq },
			{typeof(UpdateLinkedUserReq),s_UpdateLinkedUserReq },
			{typeof(GetLinkedUsersReq),s_GetLinkedUsersReq },
			{typeof(MyHackingReq),s_MyHackingReq },
			{typeof(GetDatabasesInfoReq),s_GetDatabasesInfoReq },
			{typeof(GetDatabasesUsersInfoReq),s_GetDatabasesUsersInfoReq },
			{typeof(GetDatabaseInstancesReq),s_GetDatabaseInstancesReq },
			{typeof(GetDatabaseUsersInstancesReq),s_GetDatabaseUsersInstancesReq },
			{typeof(DatabaseActionReq),s_DatabaseActionReq },
			{typeof(DatabaseMultiActionReq),s_DatabaseMultiActionReq },
			{typeof(RereadQuotaDataTablesReq),s_RereadQuotaDataTablesReq },
			{typeof(GetUserQuotaReq),s_GetUserQuotaReq },
			{typeof(SetQuotaLimit),s_SetQuotaLimit },
			{typeof(SetQuotaCurrentValue),s_SetQuotaCurrentValue },
			{typeof(ResetQuotaPeriod),s_ResetQuotaPeriod },
			{typeof(SetQuotaPeriodTimespan),s_SetQuotaPeriodTimespan },
			{typeof(SetQuotaFlags),s_SetQuotaFlags },
			{typeof(ClearQuotaFlags),s_ClearQuotaFlags },
			{typeof(SetAlertPercentage),s_SetAlertPercentage },
			{typeof(SetAlertPercentageForAllQuota),s_SetAlertPercentageForAllQuota },
			{typeof(SetDefyQuotaFlag),s_SetDefyQuotaFlag },
			{typeof(GetAllUserDescriptorsReq),s_GetAllUserDescriptorsReq },
			{typeof(ChangePrivilegeLevel),s_ChangePrivilegeLevel },
			{typeof(ChangeAccountLevel),s_ChangeAccountLevel },
			{typeof(ParkUserReq),s_ParkUserReq },
			{typeof(DeleteUserReq),s_DeleteUserReq },
			{typeof(EjectUserReq),s_EjectUserReq },
			{typeof(BanUnBanUserReq),s_BanUnBanUserReq },
			{typeof(UndeployExUserGraphReq),s_UndeployExUserGraphReq },
			{typeof(GetUserIdReq),s_GetUserIdReq },
			{typeof(ChangeSmtpPasswordRequest),s_ChangeSmtpPasswordRequest },
			{typeof(ClearRestServicesReq),s_ClearRestServicesReq },
			{typeof(ChangeDebugLevelReq),s_ChangeDebugLevelReq },
			{typeof(NodePairingRecoverNodesReq),s_NodePairingRecoverNodesReq },
			{typeof(NodePairingCompleteReq),s_NodePairingCompleteReq },
			{typeof(EnumerateSaveandDeployedGraphsReq),s_EnumerateSaveandDeployedGraphsReq },
			{typeof(GetSocialAccountsReq),s_GetSocialAccountsReq },
			{typeof(UnpairNodeReq),s_UnpairNodeReq },
			{typeof(RemoveSocialAccountReq),s_RemoveSocialAccountReq },
			{typeof(GetToolboxModelViewReq),s_GetToolboxModelViewReq },
			{typeof(GetBlockDescriptorforBlockType),s_GetBlockDescriptorforBlockType },
			{typeof(GetBlockDescriptorforThingReq),s_GetBlockDescriptorforThingReq },
			{typeof(GetBlockDescriptorforGroupReq),s_GetBlockDescriptorforGroupReq },
			{typeof(LoadGraphByFriendlyName),s_LoadGraphByFriendlyName },
			{typeof(GetAllImagesReq),s_GetAllImagesReq },
			{typeof(GetImageReq),s_GetImageReq },
			{typeof(AddResourceDescriptorRequest),s_AddResourceDescriptorRequest },
			{typeof(DeleteDescriptorRequest),s_DeleteDescriptorRequest },
			{typeof(GetAllPortKeys),s_GetAllPortKeys },
			{typeof(GetAllPorts),s_GetAllPorts },
			{typeof(PortConfigurationReq),s_PortConfigurationReq },
			{typeof(CreateMatchingVirtualThingReq),s_CreateMatchingVirtualThingReq },
			{typeof(BRDUploadReq),s_BRDUploadReq },
			{typeof(GetBinaryResourceDesciptorsRequest),s_GetBinaryResourceDesciptorsRequest },
			{typeof(CreateSnapshotReq),s_CreateSnapshotReq },
			{typeof(LoadGraphsReq),s_LoadGraphsReq },
			{typeof(LoadFilesReq),s_LoadFilesReq },
			{typeof(DeleteUploadedFileReq),s_DeleteUploadedFileReq },
			{typeof(RequestExternalGraphAction),s_RequestExternalGraphAction },
			{typeof(GetAllAvailableRestServicesReq),s_GetAllAvailableRestServicesReq },
			{typeof(GraphActionsReq),s_GraphActionsReq },
			{typeof(ShareGraphActionReq),s_ShareGraphActionReq },
			{typeof(SharedGraphConfigReq),s_SharedGraphConfigReq },
			{typeof(ConfigureUserTimezoneReq),s_ConfigureUserTimezoneReq },
			{typeof(ConfigureUserMetadataReq),s_ConfigureUserMetadataReq },
			{typeof(NotificationMarkAsRead),s_NotificationMarkAsRead },
			{typeof(NotificationMarkAllAsRead),s_NotificationMarkAllAsRead },
			{typeof(GetAllNotificationsReq),s_GetAllNotificationsReq },
			{typeof(AddWizardRestServiceReq),s_AddWizardRestServiceReq },
			{typeof(AddWizardConfiguredRestServiceReq),s_AddWizardConfiguredRestServiceReq },
			{typeof(GlobalConfiguredServiceReq),s_GlobalConfiguredServiceReq },
			{typeof(AddRestServiceReq),s_AddRestServiceReq },
			{typeof(GetAllRestServicesReq),s_GetAllRestServicesReq },
			{typeof(GetAllSRestDescriptors),s_GetAllSRestDescriptors },
			{typeof(TryGetRestServiceDescriptor),s_TryGetRestServiceDescriptor },
			{typeof(FindUserRestServiceDescriptor),s_FindUserRestServiceDescriptor },
			{typeof(RestAuth),s_RestAuth },
			{typeof(CreateFirstAuthUrl),s_CreateFirstAuthUrl },
			{typeof(AddConfiguredRestService),s_AddConfiguredRestService },
			{typeof(AddSocialAccount),s_AddSocialAccount },
						{typeof(GenericValue),s_GenericValue },
			{typeof(GetSharedThingsRsp),s_GetSharedThingsRsp },
			{typeof(NodeDescriptorsRsp),s_NodeDescriptorsRsp },
			{typeof(NodeCreateRsp),s_NodeCreateRsp },
			{typeof(GenericRsp),s_GenericRsp },
			{typeof(GetThingTypesRsp),s_GetThingTypesRsp },
			{typeof(GroupsRsp),s_GroupsRsp },
			{typeof(CreateGroupRsp),s_CreateGroupRsp },
			{typeof(MultipleThingActionRsp),s_MultipleThingActionRsp },
			{typeof(ScanRsp),s_ScanRsp },
			{typeof(GetThingKeyRsp),s_GetThingKeyRsp },
			{typeof(GetPortResp),s_GetPortResp },
			{typeof(GetThingsRsp),s_GetThingsRsp },
			{typeof(ShareThingsRsp),s_ShareThingsRsp },
			{typeof(GetUserDescriptorRsp),s_GetUserDescriptorRsp },
			{typeof(SetUserSettingsRsp),s_SetUserSettingsRsp },
			{typeof(ApiKeyRsp),s_ApiKeyRsp },
			{typeof(OAuthGetTokenResponse),s_OAuthGetTokenResponse },
			{typeof(GetFriendsRsp),s_GetFriendsRsp },
			{typeof(CreateApiKeyRsp),s_CreateApiKeyRsp },
			{typeof(GetUserTokensRsp),s_GetUserTokensRsp },
			{typeof(GetLinkedUserDescriptors),s_GetLinkedUserDescriptors },
			{typeof(MyHackingRsp),s_MyHackingRsp },
			{typeof(GetDatabasesInfoRsp),s_GetDatabasesInfoRsp },
			{typeof(GetDatabasesUsersInfoRsp),s_GetDatabasesUsersInfoRsp },
			{typeof(DatabaseMultiActionRsp),s_DatabaseMultiActionRsp },
			{typeof(GetUserQuotaRsp),s_GetUserQuotaRsp },
			{typeof(GetAllUserDescriptorsRsp),s_GetAllUserDescriptorsRsp },
			{typeof(NodePairingRecoverRsp),s_NodePairingRecoverRsp },
			{typeof(NodePairingCompetionRsp),s_NodePairingCompetionRsp },
			{typeof(DashboardGraphsResponse),s_DashboardGraphsResponse },
			{typeof(SocialAccountsRsp),s_SocialAccountsRsp },
			{typeof(GetToolboxModelViewRsp),s_GetToolboxModelViewRsp },
			{typeof(GetBlockDescriptorRsp),s_GetBlockDescriptorRsp },
			{typeof(BookmarkableLoadRoute),s_BookmarkableLoadRoute },
			{typeof(BinaryResourceDescRsp),s_BinaryResourceDescRsp },
			{typeof(GetAllPortKeysRsp),s_GetAllPortKeysRsp },
			{typeof(GetAllPortsRsp),s_GetAllPortsRsp },
			{typeof(GrafanaSnapshotCreateResponse),s_GrafanaSnapshotCreateResponse },
			{typeof(LoadGraphsResp),s_LoadGraphsResp },
			{typeof(LoadFilesResp),s_LoadFilesResp },
			{typeof(MultiGraphActionRsp),s_MultiGraphActionRsp },
			{typeof(SharedGraphConfRsp),s_SharedGraphConfRsp },
			{typeof(GetUpdatedUserDescRsp),s_GetUpdatedUserDescRsp },
			{typeof(GetNotificationsRsp),s_GetNotificationsRsp },
			{typeof(GlobalConfiguredServiceResp),s_GlobalConfiguredServiceResp },
			{typeof(RestServiceRsp),s_RestServiceRsp },
			{typeof(SocialAccountRsp),s_SocialAccountRsp },
			};

		        /// <summary>
        /// Dictionary that maps API names to classes. These names are the ones used for REST routes, MQTT topics, or RabbitMQ queue names
        /// </summary>
        //public static Dictionary<String, Type> ApiMsgNamesToTypes = ApiMsgNames.Select(e => new KeyValuePair<string, Type>(e.Value, e.Key)).ToDictionary();

		 public static Dictionary<Type, eNodePermissions> PermissionMatrix = new Dictionary<Type, eNodePermissions>()
        {
			            {typeof(GetThingsReq),eNodePermissions.AllowUserQueries},			
			            {typeof(ShareThingsReq),eNodePermissions.AllowSharingCtrl},			
			            {typeof(GetUserDescriptorReq),eNodePermissions.AllowUserQueries},			
			            {typeof(AddFriendReq),eNodePermissions.AllowUserQueries},			
			            {typeof(GetFriendsReq),eNodePermissions.AllowUserQueries},			
			            {typeof(GetUserTokensReq),eNodePermissions.AllowUserQueries},			
			            {typeof(LiveKeysAddMsg),eNodePermissions.AllowGraphCtrl},			
			            {typeof(LiveKeysDelMsg),eNodePermissions.AllowGraphCtrl},			
			            {typeof(MyHackingReq),eNodePermissions.AllowGraphCtrl},			
			            {typeof(GraphActionsReq),eNodePermissions.AllowGraphCtrl},			
			};
 static WarlockAPI()
        {
            ExtendWarlockProtocol();
        }

        static partial void ExtendWarlockProtocol();

	}
	}
		
