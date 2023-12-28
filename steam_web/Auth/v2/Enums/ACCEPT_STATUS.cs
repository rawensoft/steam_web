namespace SteamWeb.Auth.v2.Enums;

public enum ACCEPT_STATUS : byte
{
    Error,
    BadSession,
    NeedAuth,
    Success,
}
