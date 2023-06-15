namespace SteamWeb.Auth.v1.Enums;

public enum LoginResult : byte
{
    LoginOkay,
    GeneralFailure,
    BadRSA,
    BadCredentials,
    NeedCaptcha,
    Need2FA,
    NeedEmail,
    TooManyFailedLogins,
    SessionIDNotFound,
}
