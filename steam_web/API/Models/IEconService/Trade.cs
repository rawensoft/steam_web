using SteamWeb.API.Enums;
using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IEconService;
public class Trade
{
	/// <summary>
	/// a unique identifier for the trade offer
	/// </summary>
	[JsonPropertyName("tradeofferid")] public ulong TradeOfferId { get; init; }
	/// <summary>
	/// your partner in the trade offer
	/// </summary>
	[JsonPropertyName("accountid_other")] public uint AccountIdOther { get; init; }
	/// <summary>
	/// a message included by the creator of the trade offer
	/// </summary>
	[JsonPropertyName("message")] public string? Message { get; init; }
	/// <summary>
	/// unix time when the offer will expire (or expired, if it is in the past)
	/// </summary>
	[JsonPropertyName("expiration_time")] public int ExpirationTime { get; init; }
	[JsonPropertyName("trade_offer_state")] public ETradeOfferState TradeOfferState { get; init; } = ETradeOfferState.k_ETradeOfferStateInvalid;
	[JsonPropertyName("items_to_receive")] public TradeAssets[] ItemsToReceive { get; init; } = Array.Empty<TradeAssets>();
	[JsonPropertyName("items_to_give")] public TradeAssets[] ItemsToGive { get; init; } = Array.Empty<TradeAssets>();
	/// <summary>
	/// boolean to indicate this is an offer you created.
	/// </summary>
	[JsonPropertyName("is_our_offer")] public bool IsOurOffer { get; init; }
	/// <summary>
	/// unix timestamp of the time the offer was sent
	/// </summary>
	[JsonPropertyName("time_created")] public int TimeCreated { get; init; }
	/// <summary>
	/// unix timestamp of the time the trade_offer_state last changed.
	/// </summary>
	[JsonPropertyName("time_updated")] public int TimeUpdated { get; init; }
	/// <summary>
	/// ID появится при trade_offer_state == k_ETradeOfferStateAccepted
	/// </summary>
	[JsonPropertyName("tradeid")] public ulong TradeId { get; init; }
	/// <summary>
	/// boolean to indicate this is an offer automatically created from a real-time trade.
	/// </summary>
	[JsonPropertyName("from_real_time_trade")] public bool FromRealTimeTrade { get; init; }
	/// <summary>
	///  unix timestamp of when the trade hold period is supposed to be over for this trade offer
	/// </summary>
	[JsonPropertyName("escrow_end_date")] public int EscrowEndDate { get; init; }
	/// <summary>
	///  the confirmation method that applies to the user asking about the offer.
	/// </summary>
	[JsonPropertyName("confirmation_method")] public ETradeOfferConfirmationMethod ConfirmationMethod { get; init; } = ETradeOfferConfirmationMethod.k_ETradeOfferConfirmationMethod_Invalid;

	/// <summary>
	/// Трейд содержит предметы только для отправки
	/// </summary>
	[JsonIgnore] public bool OnlyGiven => ItemsToGive.Length != 0 && ItemsToReceive.Length == 0;
	/// <summary>
	/// Трейд содержит предметы только для меня
	/// </summary>
	[JsonIgnore] public bool OnlyReceive => ItemsToReceive.Length != 0 && ItemsToGive.Length == 0;
	/// <summary>
	/// Это полноценный обмен. С обеих сторон имеются предметы.
	/// </summary>
	[JsonIgnore] public bool IsExchange => ItemsToGive.Length != 0 && ItemsToReceive.Length != 0;
}