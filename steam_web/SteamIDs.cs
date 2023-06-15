using SteamWeb.Script.DTO.Listinging;
using SteamWeb.Auth.Interfaces;

namespace SteamWeb;
public static class SteamIDs
{
    /// <summary>
    /// Key - Market Hash Name, Value - SteamID
    /// </summary>
    private static Dictionary<string, string> mSteamIDs = new(30000);
    private static bool _isKeepInRAM = false;
    /// <summary>
    /// Хранить и читать данные в оперативной памяти, а не в файле
    /// </summary>
    public static bool IsKeepInRAM
    {
        get => _isKeepInRAM;
        set
        {
            _isKeepInRAM = value;
            mSteamIDs.Clear();
        }
    }
    public static string PathToSteamIDsFile { get; private set; } = Environment.CurrentDirectory + "\\steam_ids";

    /// <summary>
    /// Изменяет папку, где хранится файл со steam_id
    /// </summary>
    /// <param name="folder"></param>
    public static void SetFolder(string folder)
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT) PathToSteamIDsFile = !folder.EndsWith('\\') ? folder + "\\steam_ids" : folder + "steam_ids";
        else if (Environment.OSVersion.Platform == PlatformID.Unix) PathToSteamIDsFile = !folder.EndsWith('/') ? folder + "/steam_ids" : folder + "steam_ids";
    }
    public static void LoadSteamIDs()
    {
        if (IsKeepInRAM)
        {
            if (!File.Exists(PathToSteamIDsFile)) return;
            try
            {
                var data = File.ReadAllLines(PathToSteamIDsFile);
                mSteamIDs.Clear();
                foreach (var item in data)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    var splitted = item.Split('=');
                    var name = splitted[0];
                    if (!mSteamIDs.ContainsKey(name))
                        mSteamIDs.Add(name, splitted[1]);
                }
            }
            catch (Exception e)
            { }
        }
        else mSteamIDs.Clear();
    }
    /// <summary>
    /// Сохраняет текущие id предметов из памяти в файл
    /// </summary>
    /// <param name="isClearSave">Нужно ли записать в файл, а не добавить в файл</param>
    public static void SaveSteamIDs(bool isClearSave)
    {
        var list = new List<string>(mSteamIDs.Count);
        foreach (var item in mSteamIDs)
            list.Add($"{item.Key}={item.Value}");
        if (isClearSave) File.WriteAllLines(PathToSteamIDsFile, list);
        else File.AppendAllLines(PathToSteamIDsFile, list);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="listing"></param>
    /// <param name="Session"></param>
    /// <param name="Proxy"></param>
    /// <returns>True - взято из файла, Null если неудалось загрузить страницу с предметом</returns>
    public static async Task<(bool, string)> GetItemIDAsync(ISessionProvider Session, Web.Proxy Proxy, ListingItem listing)
    => await GetItemIDAsync(Session, Proxy, listing.AppID, listing.MarketHashName);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="appid"></param>
    /// <param name="market_hash_name"></param>
    /// <param name="Session"></param>
    /// <param name="Proxy"></param>
    /// <returns>True - взято из файла, Null если неудалось загрузить страницу с предметом</returns>
    public static async Task<(bool, string)> GetItemIDAsync(ISessionProvider Session, Web.Proxy Proxy, uint appid, string market_hash_name)
    {
        var items = mSteamIDs;
        if (items.ContainsKey(market_hash_name))
            return (true, items[market_hash_name]);
        if (!IsKeepInRAM)
        {
            var itemid = GetItemID(market_hash_name);
            return (true, itemid);
        }
        var market_item = await Steam.GetMarketItemAsync(Session, Proxy, appid, market_hash_name);
        if (market_item.item_nameid != null)
        {
            AddItemID(market_hash_name, market_item.item_nameid);
            return (false, market_item.item_nameid);
        }
        return (false, null);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="listing"></param>
    /// <param name="Session"></param>
    /// <param name="Proxy"></param>
    /// <returns>True - взято из файла, Null если неудалось загрузить страницу с предметом</returns>
    public static (bool, string) GetItemID(ISessionProvider Session, Web.Proxy Proxy, ListingItem listing) =>
        GetItemID(Session, Proxy, listing.AppID, listing.MarketHashName);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="appid"></param>
    /// <param name="market_hash_name"></param>
    /// <param name="Session"></param>
    /// <param name="Proxy"></param>
    /// <returns>True - взято из файла, Null если неудалось загрузить страницу с предметом</returns>
    public static (bool, string) GetItemID(ISessionProvider Session, Web.Proxy Proxy, uint appid, string market_hash_name)
    {
        var items = mSteamIDs;
        if (items.ContainsKey(market_hash_name))
            return (true, items[market_hash_name]);
        if (!IsKeepInRAM)
        {
            var itemid = GetItemID(market_hash_name);
            return (true, itemid);
        }
        var market_item = Steam.GetMarketItem(Session, Proxy, appid, market_hash_name);
        if (market_item.item_nameid != null)
        {
            AddItemID(market_hash_name, market_item.item_nameid);
            return (false, market_item.item_nameid);
        }
        return (false, null);
    }
    
    
    private static string GetItemID(string market_hash_name)
    {
        if (File.Exists(PathToSteamIDsFile))
        {
            try
            {
                var data = File.ReadAllLines(PathToSteamIDsFile);
                foreach (var item in data)
                {
                    if (string.IsNullOrEmpty(item))
                        continue;
                    string[] splitted = item.Split('=');
                    if (splitted[0] != market_hash_name)
                        continue;
                    return splitted[1];
                }
            }
            catch (Exception) { }
        }
        return null;
    }
    private static void AddItemID(string market_hash_name, string id)
    {
        try
        {
            File.AppendAllLines(PathToSteamIDsFile, new string[1] { $"{market_hash_name}={id}" });
        }
        catch (Exception)
        { }
    }
}
