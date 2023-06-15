using SteamWeb.API.Enums;
using System.Text.Json.Serialization;

namespace SteamWeb.API.Models.IEconService
{
    public class Trade
    {
        /// <summary>
        /// a unique identifier for the trade offer
        /// </summary>
        public string tradeofferid { get; init; }
        /// <summary>
        /// your partner in the trade offer
        /// </summary>
        public uint accountid_other { get; init; }
        /// <summary>
        /// a message included by the creator of the trade offer
        /// </summary>
        public string message { get; init; }
        /// <summary>
        /// unix time when the offer will expire (or expired, if it is in the past)
        /// </summary>
        public int expiration_time { get; init; }
        public ETradeOfferState trade_offer_state { get; init; } = ETradeOfferState.k_ETradeOfferStateInvalid;
        public TradeAssets[] items_to_receive { get; init; } = new TradeAssets[0];
        public TradeAssets[] items_to_give { get; init; } = new TradeAssets[0];
        /// <summary>
        /// boolean to indicate this is an offer you created.
        /// </summary>
        public bool is_our_offer { get; init; }
        /// <summary>
        /// unix timestamp of the time the offer was sent
        /// </summary>
        public int time_created { get; init; }
        /// <summary>
        /// unix timestamp of the time the trade_offer_state last changed.
        /// </summary>
        public int time_updated { get; init; }
        /// <summary>
        /// ID появится при trade_offer_state == k_ETradeOfferStateAccepted
        /// </summary>
        public string tradeid { get; init; }
        /// <summary>
        /// boolean to indicate this is an offer automatically created from a real-time trade.
        /// </summary>
        public bool from_real_time_trade { get; init; }
        /// <summary>
        ///  unix timestamp of when the trade hold period is supposed to be over for this trade offer
        /// </summary>
        public int escrow_end_date { get; init; }
        /// <summary>
        ///  the confirmation method that applies to the user asking about the offer.
        /// </summary>
        public ETradeOfferConfirmationMethod confirmation_method { get; init; } = ETradeOfferConfirmationMethod.k_ETradeOfferConfirmationMethod_Invalid;

        /// <summary>
        /// a unique identifier for the trade offer
        /// </summary>
        [JsonIgnore]
        public ulong u_tradeofferid
        {
            get
            {
                if (ulong.TryParse(tradeofferid, out var result)) return result;
                return 0;
            }
        }
        [JsonIgnore]
        public ulong u_tradeid
        {
            get
            {
                if (ulong.TryParse(tradeid, out var result)) return result;
                return 0;
            }
        }
        /// <summary>
        /// Трейд содержит предметы только для отправки
        /// </summary>
        [JsonIgnore]
        public bool only_given
        {
            get
            {
                if (items_to_give.Length > 0 && items_to_receive.Length == 0) return true;
                return false;
            }
        }
        /// <summary>
        /// Трейд содержит предметы только для меня
        /// </summary>
        [JsonIgnore]
        public bool only_receive
        {
            get
            {
                if (items_to_receive.Length > 0 && items_to_give.Length == 0) return true;
                return false;
            }
        }
        /// <summary>
        /// Это полноценный обмен. С обеих сторон имеются предметы.
        /// </summary>
        [JsonIgnore]
        public bool is_exchange
        {
            get
            {
                if (items_to_give.Length > 0 && items_to_receive.Length > 0)
                    return true;
                return false;
            }
        }
    }
}
