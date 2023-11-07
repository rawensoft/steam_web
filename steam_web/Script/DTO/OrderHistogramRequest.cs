using SteamWeb.Auth.Interfaces;

namespace SteamWeb.Script.DTO;
public class OrderHistogramRequest
{
    public ISessionProvider? Session { get; set; }
    public System.Net.IWebProxy? Proxy { get; set; }
    public uint AppID { get; set; }
    public string Market_Hash_Name { get; set; }
    public uint Item_Nameid { get; set; }
    public string Country { get; set; } = "RU";
    public string Language { get; set; } = "english";
    public int Currency { get; set; } = 5;
    public string Two_Factor { get; set; } = "0";
    public int Timeout { get; set; } = 0;
	internal CancellationToken? CancellationToken { get; private set; } = null;

	public OrderHistogramRequest(ISessionProvider? session, System.Net.IWebProxy? proxy, uint appid, string market_hash_name, uint item_nameid)
	{
		Session = session;
		Proxy = proxy;
		AppID = appid;
		Market_Hash_Name = market_hash_name;
		Item_Nameid = item_nameid;
	}
	public OrderHistogramRequest(ISessionProvider? session, System.Net.IWebProxy? proxy, uint appid, string market_hash_name, uint item_nameid, CancellationToken cts)
		: this(session, proxy, appid, market_hash_name, item_nameid) => CancellationToken = cts;
}
