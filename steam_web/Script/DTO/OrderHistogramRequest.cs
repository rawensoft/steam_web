using SteamWeb.Auth.Interfaces;

namespace SteamWeb.Script.DTO;
public class OrderHistogramRequest
{
    public ISessionProvider? Session { get; set; }
    public System.Net.IWebProxy? Proxy { get; set; }
    public uint AppId { get; set; }
    public string MarketHashName { get; set; }
    public ulong Item_NameId { get; set; }
    public string Country { get; set; } = "RU";
    public string Language { get; set; } = "english";
    public int Currency { get; set; } = 5;
    public string TwoFactor { get; set; } = "0";
    public int Timeout { get; set; } = 0;
    public CancellationToken? CancellationToken { get; private set; } = null;

	public OrderHistogramRequest(ISessionProvider? session, System.Net.IWebProxy? proxy, uint appid, string market_hash_name, ulong item_nameid)
	{
		Session = session;
		Proxy = proxy;
		AppId = appid;
		MarketHashName = market_hash_name;
		Item_NameId = item_nameid;
	}
	public OrderHistogramRequest(ISessionProvider? session, System.Net.IWebProxy? proxy, uint appid, string market_hash_name, ulong item_nameid, CancellationToken? cts)
		: this(session, proxy, appid, market_hash_name, item_nameid) => CancellationToken = cts;
}