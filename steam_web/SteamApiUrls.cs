namespace SteamWeb;
internal static class SteamApiUrls
{
    public const string IMobileNotificationService_GetUserNotificationCounts_v1 = "https://api.steampowered.com/IMobileNotificationService/GetUserNotificationCounts/v0001";
    public const string IGameServersService_GetAccountList_v1 = "https://api.steampowered.com/IGameServersService/GetAccountList/v1";
    public const string IUserAccountService_GetClientWalletDetails_v1 = "https://api.steampowered.com/IUserAccountService/GetClientWalletDetails/v1";
    public const string ISteamNotificationService_GetSteamNotifications_v1 = "https://api.steampowered.com/ISteamNotificationService/GetSteamNotifications/v1";
    public const string ISteamApps_UpToDateCheck_v1 = "https://api.steampowered.com/ISteamApps/UpToDateCheck/v1";

    public const string IStoreSalesService_SetVote_v1 = "https://api.steampowered.com/IStoreSalesService/SetVote/v1";

    public const string IMobileAppService_GetMobileSummary_v1 = "https://api.steampowered.com/IMobileAppService/GetMobileSummary/v1";

    public const string ISteamAwardsService_Nominate_v1 = "https://api.steampowered.com/ISteamAwardsService/Nominate/v1";
    public const string ISteamAwardsService_GetNominationRecommendations_v1 = "https://api.steampowered.com/ISteamAwardsService/GetNominationRecommendations/v1";

    public const string IStoreService_GetDiscoveryQueue_v1 = "https://api.steampowered.com/IStoreService/GetDiscoveryQueue/v1";
    public const string IStoreService_SkipDiscoveryQueueItem_v1 = "https://api.steampowered.com/IStoreService/SkipDiscoveryQueueItem/v1";

    public const string ILoyaltyRewardsService_GetSummary_v1 = "https://api.steampowered.com/ILoyaltyRewardsService/GetSummary/v1";
	public const string ILoyaltyRewardsService_GetReactionConfig_v1 = "https://api.steampowered.com/ILoyaltyRewardsService/GetReactionConfig/v1";
	public const string ILoyaltyRewardsService_AddReaction_v1 = "https://api.steampowered.com/ILoyaltyRewardsService/AddReaction/v1";

	public const string IAuthenticationService_GetPasswordRSAPublicKey_v1 = "https://api.steampowered.com/IAuthenticationService/GetPasswordRSAPublicKey/v1";
	public const string IAuthenticationService_BeginAuthSessionViaCredentials_v1 = "https://api.steampowered.com/IAuthenticationService/BeginAuthSessionViaCredentials/v1";
	public const string IAuthenticationService_UpdateAuthSessionWithSteamGuardCode_v1 = "https://api.steampowered.com/IAuthenticationService/UpdateAuthSessionWithSteamGuardCode/v1";
	public const string IAuthenticationService_PollAuthSessionStatus_v1 = "https://api.steampowered.com/IAuthenticationService/PollAuthSessionStatus/v1";
    public const string IAuthenticationService_GetAuthSessionInfo_v1 = "https://api.steampowered.com/IAuthenticationService/GetAuthSessionInfo/v1";

    public const string ITwoFactorService_QueryStatus_v1 = "https://api.steampowered.com/ITwoFactorService/QueryStatus/v1";
	public const string ITwoFactorService_AddAuthenticator_v1 = "https://api.steampowered.com/ITwoFactorService/AddAuthenticator/v1";
	public const string ITwoFactorService_FinalizeAddAuthenticator_v1 = "https://api.steampowered.com/ITwoFactorService/FinalizeAddAuthenticator/v1";
    public const string ITwoFactorService_RemoveAuthenticator_v1 = "https://api.steampowered.com/ITwoFactorService/RemoveAuthenticator/v1";
    public const string ITwoFactorService_RemoveAuthenticatorViaChallengeStart_v1 = "https://api.steampowered.com/ITwoFactorService/RemoveAuthenticatorViaChallengeStart/v1/";
	public const string ITwoFactorService_RemoveAuthenticatorViaChallengeContinue_v1 = "https://api.steampowered.com/ITwoFactorService/RemoveAuthenticatorViaChallengeContinue/v1/";

	public const string IPhoneService_SetAccountPhoneNumber_v1 = "https://api.steampowered.com/IPhoneService/SetAccountPhoneNumber/v1";
	public const string IPhoneService_IsAccountWaitingForEmailConfirmation_v1 = "https://api.steampowered.com/IPhoneService/IsAccountWaitingForEmailConfirmation/v1";
	public const string IPhoneService_SendPhoneVerificationCode_v1 = "https://api.steampowered.com/IPhoneService/SendPhoneVerificationCode/v1";
	public const string IPhoneService_VerifyAccountPhoneWithCode_v1 = "https://api.steampowered.com/IPhoneService/VerifyAccountPhoneWithCode/v1";

    public const string IPlayerService_GetProfileCustomization_v1 = "https://api.steampowered.com/IPlayerService/GetProfileCustomization/v1";
    public const string IPlayerService_SetProfileTheme_v1 = "https://api.steampowered.com/IPlayerService/SetProfileTheme/v1";
    public const string IPlayerService_GetBadges_v1 = "https://api.steampowered.com/IPlayerService/GetBadges/v1";
    public const string IPlayerService_GetCommunityBadgeProgress_v1 = "https://api.steampowered.com/IPlayerService/GetCommunityBadgeProgress/v1";
    public const string IPlayerService_GetFavoriteBadge_v1 = "https://api.steampowered.com/IPlayerService/GetFavoriteBadge/v1";
    public const string IPlayerService_GetOwnedGames_v1 = "https://api.steampowered.com/IPlayerService/GetOwnedGames/v1";
    public const string IPlayerService_GetSteamLevel_v1 = "https://api.steampowered.com/IPlayerService/GetSteamLevel/v1";
    public const string IPlayerService_GetNicknameList_v1 = "https://api.steampowered.com/IPlayerService/GetNicknameList/v1";

    public const string ICSGOServers_730_GetGameServersStatus_v1 = "https://api.steampowered.com/ICSGOServers_730/GetGameServersStatus/v1";
    public const string ICSGOPlayers_730_GetNextMatchSharingCode_v1 = "https://api.steampowered.com/ICSGOPlayers_730/GetNextMatchSharingCode/v1";

    public const string IEconService_GetTradeHistory_v1 = "https://api.steampowered.com/IEconService/GetTradeHistory/v1";
    public const string IEconService_GetTradeHoldDurations_v1 = "https://api.steampowered.com/IEconService/GetTradeHoldDurations/v1";
    public const string IEconService_GetTradeOffer_v1 = "https://api.steampowered.com/IEconService/GetTradeOffer/v1";
    public const string IEconService_GetTradeOffers_v1 = "https://api.steampowered.com/IEconService/GetTradeOffers/v1";
    public const string IEconService_GetTradeOffersSummary_v1 = "https://api.steampowered.com/IEconService/GetTradeOffersSummary/v1";
    public const string IEconService_GetTradeStatus_v1 = "https://api.steampowered.com/IEconService/GetTradeStatus/v1";
    public const string IEconService_CancelTradeOffer_v1 = "https://api.steampowered.com/IEconService/CancelTradeOffer/v1";
    public const string IEconService_DeclineTradeOffer_v1 = "https://api.steampowered.com/IEconService/DeclineTradeOffer/v1";

    public const string ISteamUserOAuth_GetTokenDetails = "https://api.steampowered.com/ISteamUserOAuth/GetTokenDetails/v1";
    public const string ISteamUserOAuth_GetGroupList_v1 = "https://api.steampowered.com/ISteamUserOAuth/GetGroupList/v1";
    public const string ISteamUserOAuth_GetFriendList_v1 = "https://api.steampowered.com/ISteamUserOAuth/GetFriendList/v1";

    public const string ISteamUser_GetPlayerSummaries_v2 = "https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2";
    public const string ISteamUser_GetPlayerBans_v1 = "https://api.steampowered.com/ISteamUser/GetPlayerBans/v1";
    public const string ISteamUser_GetFriendList_v1 = "https://api.steampowered.com/ISteamUser/GetFriendList/v1";
    public const string ISteamUser_ResolveVanityURL_v1 = "https://api.steampowered.com/ISteamUser/ResolveVanityURL/v1";

    public const string IFriendMessagesService_GetActiveMessageSessions_v1 = "https://api.steampowered.com/IFriendMessagesService/GetActiveMessageSessions/v1";
    public const string IFriendMessagesService_GetRecentMessages_v1 = "https://api.steampowered.com/IFriendMessagesService/GetRecentMessages/v0001";
    public const string IFriendMessagesService_MarkOfflineMessagesRead_v1 = "https://api.steampowered.com/IFriendMessagesService/MarkOfflineMessagesRead/v1";

    public const string IFriendsListService_GetFavorites_v1 = "https://api.steampowered.com/IFriendsListService/GetFavorites/v1";
    public const string IFriendsListService_GetFriendsList_v1 = "https://api.steampowered.com/IFriendsListService/GetFriendsList/v1";

    public const string ISteamEconomy_GetAssetClassInfo_v1 = "https://api.steampowered.com/ISteamEconomy/GetAssetClassInfo/v1";
    public const string ISteamEconomy_GetAssetPrices_v1 = "https://api.steampowered.com/ISteamEconomy/GetAssetPrices/v1";

    public const string IAuthenticationService_GetAuthSessionsForAccount_v1 = "https://api.steampowered.com/IAuthenticationService/GetAuthSessionsForAccount/v1";
    public const string IAuthenticationService_GenerateAccessTokenForApp_v1 = "https://api.steampowered.com/IAuthenticationService/GenerateAccessTokenForApp/v1";
    public const string IAuthenticationService_UpdateAuthSessionWithMobileConfirmation_v1 = "https://api.steampowered.com/IAuthenticationService/UpdateAuthSessionWithMobileConfirmation/v1";
}
