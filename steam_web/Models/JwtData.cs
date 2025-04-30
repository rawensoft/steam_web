using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using SteamWeb.Extensions;

namespace SteamWeb.Models;
public class JwtData
{
    public const string Web = "web";
    public const string Renew = "renew";
    public const string Derive = "derive";
    public const string WebCommunity = "web:community";
    public const string WebStore = "web:store";
    public const string WebCheckout = "web:checkout";
    public const string WebHelp = "web:help";
    public const string Client = "client";
    public const string Mobile = "mobile";

    [JsonPropertyName("iss")] public string Issuer { get; init; }
    [JsonPropertyName("sub")] public ulong Subject { get; init; }
    [JsonPropertyName("aud")] public List<string> Audience { get; init; } = new(5);
    [JsonPropertyName("exp")] public int ExpirationAt { get; init; }
    [JsonPropertyName("nbf")] public int NotBefore { get; init; }
    [JsonPropertyName("iat")] public int IssuedAt { get; init; }
    [JsonPropertyName("jti")] public string JwtId { get; init; }
    [JsonPropertyName("oat")] public int OAt { get; init; }
    [JsonPropertyName("rt_exp")] public int? RefreshTokenExpire { get; init; }
    [JsonPropertyName("per")] public byte Version { get; init; }
    [JsonPropertyName("ip_subject")] public string IpSubject { get; init; }
    [JsonPropertyName("ip_confirmer")] public string IpConfirmer { get; init; }

    /// <summary>
    /// Десерилизует куки steamLoginSecure или steamRefresh_steam и токены access или refresh
    /// </summary>
    /// <param name="data">Кука или токен</param>
    /// <returns>Объект с информацией об этом токене</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="JsonException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public static JwtData Deserialize(string data)
    {
        if (data.StartsWith('7'))
        {
            data = HttpUtility.UrlDecode(data);
            var splitted = data.Split('|');
            if (splitted.Length != 3)
                throw new ArgumentException("Не удалось получить часто токена через разделение |", nameof(data));
            data = splitted[2];
        }
        var spl = data.Split('.');

        if (spl.Length != 3)
            throw new ArgumentException("Не удалось разделить jwt", nameof(data));

        var bytes = Convert.FromBase64String(spl[1].FromBase64UrlToBase64());
        var jwt = JsonSerializer.Deserialize<JwtData>(bytes, new JsonSerializerOptions { NumberHandling = JsonNumberHandling.AllowReadingFromString })!;
        return jwt;
    }

    public bool HasRenewAudience()
    {
        bool flag_web_site = Audience.Contains(Renew) && Audience.Contains(Derive);
        return flag_web_site;
    }
    public bool HasCookieAudience()
    {
        bool flag_web_site = Audience.Contains(WebCommunity) || Audience.Contains(WebStore) || Audience.Contains(WebCheckout) || Audience.Contains(WebHelp);
        return flag_web_site;
    }
    public bool HasClientAudience()
    {
        bool flag_web_site = Audience.Contains(Client) && Audience.Contains(Web);
        return flag_web_site;
    }
    public bool HasAudience()
    {
        bool flag_web_site = Audience.Contains(Client) && Audience.Contains(Web);
        return flag_web_site;
    }
}