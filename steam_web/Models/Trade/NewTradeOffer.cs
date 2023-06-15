using System.Collections.Generic;
namespace SteamWeb.Models.Trade;
public sealed class NewTradeOffer
{
    public bool newversion { get; init; } = true;
    public int version { get; init; } = 4;
    public New me { get; init; } = new();
    public New them { get; init; } = new();
    public class New
    {
        public List<NewAssets> assets { get; init; } = new(10);
        public List<NewCurrency> currency { get; init; } = new(10);
        public bool ready { get; init; } = false;

        public class NewAssets
        {
            public uint appid { get; init; }
            public string contextid { get; init; }
            public int amount { get; init; }
            public string assetid { get; init; }
            public NewAssets(uint appid, string contextid, string assetid, int amount = 1)
            {
                this.appid = appid;
                this.contextid = contextid;
                this.amount = amount;
                this.assetid = assetid;
            }
        }
        public class NewCurrency { }

        public void AddAssets(uint appid, string contextid, string assetid, int amount = 1) => assets.Add(new NewAssets(appid, contextid, assetid, amount));
    }
}
