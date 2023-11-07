namespace SteamWeb.Auth.v2.Enums;
public enum LoginResult : byte
{
    LoginOkay,
    GeneralFailure,
    BadRSA,
    BadCredentials,
    BadCookie,
    NeedAprove,
    RateExceeded,
	//NeedCaptcha,
	ProxyError,
    Timeout,
    ConnectionError
}
