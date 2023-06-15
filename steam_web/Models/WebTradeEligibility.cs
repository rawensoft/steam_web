using System.Text.Json.Serialization;
namespace SteamWeb.Models;
public sealed class WebTradeEligibility
{
    /// <summary>
    /// 1 - разрешён
    /// </summary>
    public int allowed { get; init; }
    /// <summary>
    /// 8 - limit
    /// 16 - trade ban
    /// 16416 - необходимо совершить покупку в стим
    /// 16424 - лимит и необходимо совершить покупку в стим
    /// 16800 - необходимо совершить покупку в стим, пароль был изменён, гуард был выключе
    /// 18720 - долго не входил в аккаунт и нужно совершить покупку в стим и пароль недавно изменён
    /// </summary>
    public int reason { get; init; }
    [JsonIgnore] public WebTradeEligibilityReason reason_advanced => new(this);
    public int allowed_at_time { get; init; }
    public int steamguard_required_days { get; init; }
    public int new_device_cooldown_days { get; init; }
    public long time_checked { get; init; }
    public long expiration { get; init; }
    public bool is_limited => new WebTradeEligibilityReason(this).IsLimited;
    public bool is_can_market => !new WebTradeEligibilityReason(this).CanMarket;
    public bool is_can_trade => new WebTradeEligibilityReason(this).CanTrade;
}
