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
    /// <summary>
    /// По умолчание текущая директория <see cref="Environment.CurrentDirectory"/>
    /// </summary>
    public static string PathToSteamIDsFile { get; private set; } = Path.Join(Environment.CurrentDirectory, "steam_ids");

    /// <summary>
    /// Изменяет папку, где хранится файл со steam_id
    /// </summary>
    /// <param name="folder"></param>
    public static void SetFolder(string folder)
    {
        PathToSteamIDsFile = Path.Join(folder, "steam_ids");
    }
    /// <summary>
    /// Загружает данные из файла steam_ids, указанного в <see cref="PathToSteamIDsFile"/>
    /// </summary>
    /// <returns>true данные загружены, в других случаях false (проверьте существует ли файл)</returns>
    public static bool LoadSteamIDs()
    {
        if (IsKeepInRAM)
        {
            if (!File.Exists(PathToSteamIDsFile))
                return false;
            lock (_locker)
            {
                mSteamIDs.Clear();
                try
                {
                    using var fs = new FileStream(PathToSteamIDsFile, FileMode.Open, FileAccess.Read);
                    var sr = new StreamReader(fs);
                    string? str = sr.ReadLine();
                    while (!str.IsEmpty())
                    {
                        var splitted = str!.Split('=');
                        var name = splitted[0];
                        var item_nameid = splitted[1].ParseUInt32();
                        mSteamIDs.TryAdd(name, item_nameid);

                        str = sr.ReadLine();
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        else mSteamIDs.Clear();
        return true;
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
    public static async Task<(bool, uint)> GetItemIDAsync(ISessionProvider session, Proxy? proxy, ListingItem listing, CancellationToken? cancellationToken = null)
    => await GetItemIDAsync(session, proxy, listing.AppID, listing.MarketHashName, cancellationToken);
    /// <summary>
    /// Получает item_nameid из файла или извлекает из страницы предмета, если его не оказалось в файле
    /// </summary>
    /// <param name="appid">ID приложения\\игры</param>
    /// <param name="market_hash_name">Название предмета</param>
    /// <returns>True - взято из файла, Null если неудалось загрузить страницу с предметом</returns>
    public static async Task<(bool, uint)> GetItemIDAsync(ISessionProvider session, Proxy? proxy, uint appid, string market_hash_name, CancellationToken? cancellationToken = null)
    {
        var items = mSteamIDs;
        if (items.TryGetValue(market_hash_name, out var item_nameid))
            return (true, item_nameid);
        if (!IsKeepInRAM)
        {
			item_nameid = GetItemID(market_hash_name, cancellationToken);
			return (true, item_nameid);
		}

        var market_item = await Steam.GetMarketItemAsync(new(session, proxy, cancellationToken), appid, market_hash_name);
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
    public static (bool, uint) GetItemID(ISessionProvider session, Proxy? proxy, ListingItem listing, CancellationToken? cancellationToken = null) =>
        GetItemID(session, proxy, listing.AppID, listing.MarketHashName, cancellationToken);
    /// <summary>
    /// Получает item_nameid из файла или извлекает из страницы предмета, если его не оказалось в файле
    /// </summary>
    /// <param name="appid">ID приложения\\игры</param>
    /// <param name="market_hash_name">Название предмета</param>
    /// <returns>True - взято из файла, Null если не удалось загрузить страницу с предметом</returns>
    public static (bool, uint) GetItemID(ISessionProvider session, Proxy? proxy, uint appid, string market_hash_name, CancellationToken? cancellationToken = null)
    {
        var items = mSteamIDs;
        if (items.TryGetValue(market_hash_name, out var item_nameid))
            return (true, item_nameid);
        if (!IsKeepInRAM)
        {
			item_nameid = GetItemID(market_hash_name, cancellationToken);
			return (true, item_nameid);
        }

        var market_item = Steam.GetMarketItem(new(session, proxy, cancellationToken), appid, market_hash_name);
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

    private static uint GetItemID(string market_hash_name, CancellationToken? cancellationToken = null)
    {
        if (File.Exists(PathToSteamIDsFile))
        {
            try
            {
                using var fs = new FileStream(PathToSteamIDsFile, FileMode.Open, FileAccess.Read);
                var sr = new StreamReader(fs);
                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                    return 0;
                string? str = sr.ReadLine();
                while (!str.IsEmpty())
                {
                    if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                        return 0;

                    string[] splitted = str!.Split('=');
                    str = sr.ReadLine(); // нужно читать след. строку именно здесь, после разделения текущей и до проверки hash_name
                    if (splitted[0] != market_hash_name)
                        continue;

                    return splitted[1].ParseUInt32();
                }
            }
            catch (Exception)
            {
                // пробуем открыть файл на чтение и прочитать строки
                // у нас главная задача прочитать, а остальное не важно
                // проблем, если добавится повтор item_nameid, не будет
            }
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