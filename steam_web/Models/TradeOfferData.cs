using System.Text.Json;
using SteamWeb.Extensions;
using SteamWeb.Models.Trade;

namespace SteamWeb.Models;

public class TradeOfferData
{
    private const string _stringTrue = "true";
	public const string ErrorTradeNoLongerValidEN = "This trade offer is no longer valid.";
    public const string ErrorTradeNoLongerValidRU = "Это предложение обмена больше не действительно.";
    public const string ErrorTradeDoesntExistEN = "The trade offer does not exist, or the trade offer belongs to another user.";

    public bool IsSuccess { get; internal set; } = false;
    public string? Error { get; internal set; }
    /// <summary>
    /// Трейд был принят, либо отменён
    /// </summary>
    public bool IsNoLongerValid => Error == ErrorTradeNoLongerValidEN || Error == ErrorTradeNoLongerValidRU;
    /// <summary>
    /// The trade offer does not exist, or the trade offer belongs to another user.
    /// </summary>
    public bool TradeDoesntExists => Error == ErrorTradeDoesntExistEN;

    /// <summary>
    /// Id трейда
    /// </summary>
    public ulong TradeOfferId { get; internal set; }
    /// <summary>
    /// Разрешена ли торговля на маркете
    /// </summary>
    public bool IsMarketAllowed { get; private set; } = false;
    /// <summary>
    /// SteamId64 партнёра по обмену
    /// </summary>
    public ulong PartnerSteamId64 { get; private set; }
    /// <summary>
    /// SteamId32 партнёра по обмену
    /// </summary>
    public uint PartnerSteamId32 { get; private set; }
    public bool PartnerProbation { get; private set; }
    /// <summary>
    /// Имя аккаунта партнёра по обмену
    /// </summary>
    public string? PartnerName { get; private set; }
    /// <summary>
    /// Ваш SteamId64
    /// </summary>
    public ulong YouSteamId64 { get; private set; }
    /// <summary>
    /// Ваш SteamId32
    /// </summary>
    public uint YouSteamId32 { get; private set; }
    /// <summary>
    /// Ваше имя аккаунта
    /// </summary>
    public string? YouName { get; private set; }
    public string? SessionId { get; private set; }
    /// <summary>
    /// Данные трейда; доступны если трейд активен
    /// </summary>
    public NewTradeOffer TradeStatus { get; private set; } = new();

    internal bool SetTradeStatus(string? data)
    {
        if (data.IsEmpty())
            return false;
        try
        {
            var options = new JsonSerializerOptions
            {
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
            };
            TradeStatus = JsonSerializer.Deserialize<NewTradeOffer>(data!, options)!;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    internal void SetPartner(string? name, string probation, string? steamId64)
    {
        PartnerName = name;
        PartnerProbation = probation == _stringTrue;
        var sid64 = steamId64.ParseUInt64();
        if (sid64 > 0)
        {
            PartnerSteamId64 = sid64;
            PartnerSteamId32 = Steam.Steam64ToSteam32(sid64);
        }
    }
    internal void SetYou(string? name, string? sessionId, string marketAllowed, string? steamId64)
    {
        YouName = name;
        SessionId = sessionId;
        IsMarketAllowed = marketAllowed == _stringTrue;
        var sid64 = steamId64.ParseUInt64();
        if (sid64 > 0)
        {
            YouSteamId64 = sid64;
            YouSteamId32 = Steam.Steam64ToSteam32(sid64);
        }
    }
}