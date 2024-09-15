using System.Text.Json.Serialization;

namespace SteamWeb.Models.Trade;
public class NewTradeOffer
{
    [JsonPropertyName("newversion")] public bool NewVersion { get; init; } = true;
    [JsonPropertyName("version")] public int Version { get; init; } = 4;
    [JsonPropertyName("me")] public New Me { get; init; } = new();
    [JsonPropertyName("them")] public New Them { get; init; } = new();

    public class New
    {
        [JsonPropertyName("assets")] public List<NewAssets> Assets { get; init; } = new(10);
        [JsonPropertyName("currency")] public List<NewCurrency> Currency { get; init; } = new(2);
        [JsonPropertyName("ready")] public bool Ready { get; init; } = false;

        public void AddAssets(uint appid, string contextid, string assetid, uint amount = 1) => Assets.Add(new NewAssets(appid, contextid, assetid, amount));
        public void AddAssets(uint appid, byte contextid, ulong assetid, uint amount = 1) => Assets.Add(new NewAssets(appid, contextid, assetid, amount));
    }
    public class NewAssets
    {
        [JsonPropertyName("appid")] public uint AppId { get; init; }
        [JsonPropertyName("contextid")] public string ContextId { get; init; }
        [JsonPropertyName("amount")] public uint Amount { get; init; }
        [JsonPropertyName("assetid")] public string AssetId { get; init; }

        [JsonConstructor]
        public NewAssets(uint AppId, string ContextId, string AssetId, uint Amount = 1)
        {
            this.AppId = AppId;
            this.ContextId = ContextId;
            this.Amount = Amount;
            this.AssetId = AssetId;
        }
        public NewAssets(uint AppId, byte ContextId, ulong AssetId, uint Amount = 1)
        {
            this.AppId = AppId;
            this.ContextId = ContextId.ToString();
            this.Amount = Amount;
            this.AssetId = AssetId.ToString();
        }
    }
    public class NewCurrency { }
}