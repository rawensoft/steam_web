using System.Text.Json.Serialization;
namespace SteamWeb.Models;
public class WebTradeEligibility
{
    /// <summary>
    /// 1 - разрешён
    /// </summary>
    [JsonPropertyName("allowed")] public byte Allowed { get; init; }
    /// <summary>
    /// 8 - limit
    /// 16 - trade ban
    /// 16416 - необходимо совершить покупку в стим
    /// 16424 - лимит и необходимо совершить покупку в стим
    /// 16800 - необходимо совершить покупку в стим, пароль был изменён, гуард был выключе
    /// 18720 - долго не входил в аккаунт и нужно совершить покупку в стим и пароль недавно изменён
    /// </summary>
    [JsonPropertyName("reason")] public uint Reason { get; init; }
    [JsonPropertyName("allowed_at_time")] public int AllowedAtTime { get; init; }
    [JsonPropertyName("steamguard_required_days")] public int SteamguardRequiredDays { get; init; }
    [JsonPropertyName("new_device_cooldown_days")] public int NewDeviceCooldownDays { get; init; }
    [JsonPropertyName("time_checked")] public long TimeChecked { get; init; }
    [JsonPropertyName("expiration")] public long Expiration { get; init; }

    [JsonIgnore] public WebTradeEligibilityReason ReasonAdvanced => new(this);
    [JsonIgnore] public bool IsLimited => new WebTradeEligibilityReason(this).IsLimited;
    [JsonIgnore] public bool IsCanMarket => !new WebTradeEligibilityReason(this).CanMarket;
    [JsonIgnore] public bool IsCanTrade => new WebTradeEligibilityReason(this).CanTrade;
}