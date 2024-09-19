using System.Text.Json.Serialization;

namespace SteamWeb.Models;
public class UserInfoModel
{
    [JsonPropertyName("logged_in")] public bool LoggedIn { get; init; } = false;
    [JsonPropertyName("steamid")] public ulong SteamId { get; init; }
    [JsonPropertyName("accountid")] public uint AccountId { get; init; }
    [JsonPropertyName("account_name")] public string AccountName { get; init; }
    [JsonPropertyName("is_support")] public bool IsSupport { get; init; } = false;
    [JsonPropertyName("is_limited")] public bool IsLimited { get; init; } = false;
    [JsonPropertyName("is_partner_member")] public bool IsPartnerMember { get; init; } = false;
    [JsonPropertyName("country_code")] public string CountryCode { get; init; }
}