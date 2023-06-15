namespace SteamWeb.Auth.v1.Enums;

public enum FinalizeResult : byte
{
    AuthenticatorNotAllowed,
    BadSMSCode,
    UnableToGenerateCorrectCodes,
    Success,
    GeneralFailure
}
