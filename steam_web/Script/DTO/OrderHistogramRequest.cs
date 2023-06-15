using SteamWeb.Auth.Interfaces;

namespace SteamWeb.Script.DTO;
public class OrderHistogramRequest
{
    public ISessionProvider Session { get; set; }
    public System.Net.IWebProxy Proxy { get; set; }
    public uint AppID { get; set; }
    public string Market_Hash_Name { get; set; }
    public string Item_Nameid { get; set; }
    public string Country { get; set; } = "RU";
    public string Language { get; set; } = "english";
    public int Currency { get; set; } = 5;
    public string Two_Factor { get; set; } = "0";
    public int Timeout { get; set; } = 0;

    public OrderHistogramRequest(ISessionProvider session, System.Net.IWebProxy proxy, uint appid, string market_hash_name, string item_nameid)
    {
        Session = session;
        Proxy = proxy;
        AppID = appid;
        Market_Hash_Name = market_hash_name;
        Item_Nameid = item_nameid;
    }
}
