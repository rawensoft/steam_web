namespace SteamWeb.Auth.v1.Enums;

public enum RESPONSE_STATUS : byte
{
    Success,
    Error,
    WGTokenInvalid,
    WGTokenExpired,
    WrongPlatform
}
