namespace SteamWeb;
internal static class SteamApiUrls
{
	public const string IAuthenticationService_GetPasswordRSAPublicKey_v1 = "https://api.steampowered.com/IAuthenticationService/GetPasswordRSAPublicKey/v1";
	public const string IAuthenticationService_BeginAuthSessionViaCredentials_v1 = "https://api.steampowered.com/IAuthenticationService/BeginAuthSessionViaCredentials/v1";
	public const string IAuthenticationService_UpdateAuthSessionWithSteamGuardCode_v1 = "https://api.steampowered.com/IAuthenticationService/UpdateAuthSessionWithSteamGuardCode/v1";
	public const string IAuthenticationService_PollAuthSessionStatus_v1 = "https://api.steampowered.com/IAuthenticationService/PollAuthSessionStatus/v1";

	public const string ITwoFactorService_QueryStatus_v1 = "https://api.steampowered.com/ITwoFactorService/QueryStatus/v1";
	public const string ITwoFactorService_AddAuthenticator_v1 = "https://api.steampowered.com/ITwoFactorService/AddAuthenticator/v1";
	public const string ITwoFactorService_FinalizeAddAuthenticator_v1 = "https://api.steampowered.com/ITwoFactorService/FinalizeAddAuthenticator/v1";

	public const string IPhoneService_SetAccountPhoneNumber_v1 = "https://api.steampowered.com/IPhoneService/SetAccountPhoneNumber/v1";
	public const string IPhoneService_IsAccountWaitingForEmailConfirmation_v1 = "https://api.steampowered.com/IPhoneService/IsAccountWaitingForEmailConfirmation/v1";
	public const string IPhoneService_SendPhoneVerificationCode_v1 = "https://api.steampowered.com/IPhoneService/SendPhoneVerificationCode/v1";
	public const string IPhoneService_VerifyAccountPhoneWithCode_v1 = "https://api.steampowered.com/IPhoneService/VerifyAccountPhoneWithCode/v1";

	public const string IAuthenticationService_UpdateAuthSessionWithMobileConfirmation_v1 = "https://api.steampowered.com/IAuthenticationService/UpdateAuthSessionWithMobileConfirmation/v1";
}
