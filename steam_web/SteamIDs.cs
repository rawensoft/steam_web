using SteamWeb.Script.DTO.Listinging;
using SteamWeb.Auth.Interfaces;
using SteamWeb.Extensions;
using SteamWeb.Web;
using System.Text;

namespace SteamWeb;
public static class SteamIDs
{
    private static readonly object _locker = new();
    /// <summary>
    /// Key - Market Hash Name, Value - SteamID
    /// </summary>
    private static Dictionary<string, uint> mSteamIDs = new(30000);
    private static bool _isKeepInRAM = false;
    /// <summary>
    /// Хранить и читать данные в оперативной памяти, а не в файле. При изменении значения очищает данные с item_nameid.
    /// </summary>
    public static bool IsKeepInRAM
    {
        get => _isKeepInRAM;
        set
        {
            _isKeepInRAM = value;
            lock(_locker)
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
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            PathToSteamIDsFile = !folder.EndsWith('\\') ? folder + "\\steam_ids" : folder + "steam_ids";
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
            PathToSteamIDsFile = !folder.EndsWith('/') ? folder + "/steam_ids" : folder + "steam_ids";
    }
    public static void LoadSteamIDs()
    {
        if (IsKeepInRAM)
        {
            if (!File.Exists(PathToSteamIDsFile))
                return;
            try
            {
                lock (_locker)
                {
                    var data = File.ReadAllLines(PathToSteamIDsFile);
                    mSteamIDs.Clear();
                    var length = data.Length;
                    for (int i = 0; i < length; i++)
                    {
                        var item = data[i];
                        if (string.IsNullOrEmpty(item))
                            continue;
                        var splitted = item.Split('=');
                        var name = splitted[0];
                        if (!mSteamIDs.ContainsKey(name))
                            mSteamIDs.Add(name, splitted[1].ParseUInt32());
                    }
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
        lock (_locker)
        {
            var sb = new StringBuilder(mSteamIDs.Count * 4 + 2);
            foreach (var item in mSteamIDs)
            {
                sb.Append(item.Key);
                sb.Append('=');
                sb.Append(item.Value);
                sb.Append('\n');
            }
            if (isClearSave)
                File.WriteAllText(PathToSteamIDsFile, sb.ToString());
            else
                File.AppendAllText(PathToSteamIDsFile, sb.ToString());
        }
    }


    /// <summary>
    /// Получает item_nameid из файла или извлекает из страницы предмета, если его не оказалось в файле
    /// </summary>
    /// <returns>True - взято из файла, Null если неудалось загрузить страницу с предметом</returns>
    public static async Task<(bool, uint)> GetItemIDAsync(ISessionProvider session, Proxy? proxy, ListingItem listing)
    => await GetItemIDAsync(session, proxy, listing.AppID, listing.MarketHashName);
    /// <summary>
    /// Получает item_nameid из файла или извлекает из страницы предмета, если его не оказалось в файле
    /// </summary>
    /// <param name="appid">ID приложения\\игры</param>
    /// <param name="market_hash_name">Название предмета</param>
    /// <returns>True - взято из файла, Null если неудалось загрузить страницу с предметом</returns>
    public static async Task<(bool, uint)> GetItemIDAsync(ISessionProvider session, Proxy? proxy, uint appid, string market_hash_name)
    {
        var items = mSteamIDs;
        if (items.TryGetValue(market_hash_name, out var item_nameid))
            return (true, item_nameid);
        if (!IsKeepInRAM)
        {
			item_nameid = GetItemID(market_hash_name);
			return (true, item_nameid);
		}

        var market_item = await Steam.GetMarketItemAsync(session, proxy, appid, market_hash_name);
        if (market_item.item_nameid != null)
        {
            AddItemID(market_hash_name, market_item.item_nameid);
            return (false, market_item.item_nameid.ParseUInt32());
        }
        return (false, 0);
    }

    /// <summary>
    /// Получает item_nameid из файла или извлекает из страницы предмета, если его не оказалось в файле
    /// </summary>
    /// <returns>True - взято из файла, Null если неудалось загрузить страницу с предметом</returns>
    public static (bool, uint) GetItemID(ISessionProvider session, Proxy? proxy, ListingItem listing) =>
        GetItemID(session, proxy, listing.AppID, listing.MarketHashName);
    /// <summary>
    /// Получает item_nameid из файла или извлекает из страницы предмета, если его не оказалось в файле
    /// </summary>
    /// <param name="appid">ID приложения\\игры</param>
    /// <param name="market_hash_name">Название предмета</param>
    /// <returns>True - взято из файла, Null если не удалось загрузить страницу с предметом</returns>
    public static (bool, uint) GetItemID(ISessionProvider session, Proxy? proxy, uint appid, string market_hash_name)
    {
        var items = mSteamIDs;
        if (items.TryGetValue(market_hash_name, out var item_nameid))
            return (true, item_nameid);
        if (!IsKeepInRAM)
        {
			item_nameid = GetItemID(market_hash_name);
			return (true, item_nameid);
		}

        var market_item = Steam.GetMarketItem(session, proxy, appid, market_hash_name);
        if (market_item.item_nameid != null)
        {
            AddItemID(market_hash_name, market_item.item_nameid);
            return (false, market_item.item_nameid?.ParseUInt32() ?? 0);
        }
        return (false, 0);
    }

    /// <summary>
    /// Получает item_nameid из кеша
    /// </summary>
    /// <param name="market_hash_name">Название предмета</param>
    /// <returns>Null если данных для этого предмета нет</returns>
    public static uint GetItemIDLocal(string market_hash_name)
    {
        if (!IsKeepInRAM)
        {
            var item_nameid = GetItemID(market_hash_name);
			return item_nameid;
		}
        else if (mSteamIDs.TryGetValue(market_hash_name, out var item_nameid))
            return item_nameid;
        return 0;
    }
    public static bool TryAddItemID(string market_hash_name, uint item_nameid, bool afterSave)
    {
        lock (_locker)
        {
            if (IsKeepInRAM)
            {
                if (!mSteamIDs.TryAdd(market_hash_name, item_nameid))
                    return false;
            }
            else
                File.AppendAllLines(PathToSteamIDsFile, new string[] { market_hash_name + '=' + item_nameid });
        }
        if (IsKeepInRAM && afterSave)
            SaveSteamIDs(true);
        return true;
    }

    private static uint GetItemID(string market_hash_name)
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
                    return splitted[1].ParseUInt32();
                }
            }
            catch (Exception) { }
        }
        return 0;
    }
    private static void AddItemID(string market_hash_name, string id)
    {
        try
        {
            lock(_locker)
                File.AppendAllLines(PathToSteamIDsFile, new string[] { market_hash_name + '=' + id });
        }
        catch (Exception)
        { }
    }
}
