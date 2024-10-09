using SteamWeb.Models;

namespace SteamWeb.Script.DTO.ItemRender;

public class ItemRenderRequest
{
    public DefaultRequest DefaultRequest { get; }
    public string MarketHashName { get; }
    public uint AppId { get; }
    public byte Currency { get; } = 5;
    public string Country { get; } = "RU";
    public string Language { get; } = "english";
    public uint Count { get; init; } = 100;
    public uint Start { get; set; } = 0;
    public string? Query { get; init; }

    public ItemRenderRequest(DefaultRequest defaultRequest, string market_hash_name, uint appId)
    {
        DefaultRequest = defaultRequest;
        MarketHashName = market_hash_name;
        AppId = appId;
    }
    public ItemRenderRequest(DefaultRequest defaultRequest, string market_hash_name, uint appId, string language, string country)
        : this(defaultRequest, market_hash_name, appId)
    {
        Country = country;
        Language = language;
    }
    public ItemRenderRequest(DefaultRequest defaultRequest, string market_hash_name, uint appId, byte currency)
        : this(defaultRequest, market_hash_name, appId) => Currency = currency;
    public ItemRenderRequest(DefaultRequest defaultRequest, string market_hash_name, uint appId, byte currency, string language, string country)
        : this(defaultRequest, market_hash_name, appId, language, country) => Currency = currency;
}