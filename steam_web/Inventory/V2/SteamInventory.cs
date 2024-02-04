using System.Text.Json;
using SteamWeb.Extensions;
using SteamWeb.Inventory.V2.Models;
using SteamWeb.Web;
using SteamWeb.Auth.Interfaces;

namespace SteamWeb.Inventory.V2;
public class SteamInventory
{
    public bool success { get; init; } = false;
    /// <summary>
    /// Key = classid_instanceid
    /// </summary>
    public Dictionary<string, Asset> rgInventory { get; init; } = new(1);
    /// <summary>
    /// Key = id
    /// </summary>
    public Dictionary<string, Description> rgDescriptions { get; init; } = new(1);
    public bool more { get; init; } = false;
    //public bool more_start { get; init; } = false;
    public string context { get; set; } = "2";
    public string? error { get; init; }
    public bool is_too_many_requests { get; init; } = false;

    public static SteamInventory Load(ISessionProvider? session, System.Net.IWebProxy? proxy, ulong steamid64, uint appid, string context = "2", bool trading = false)
    {
        string url = GetUrl(steamid64, appid, context, trading);
        try
        {
            var getRequest = new GetRequest(url, proxy, session) { IsAjax = true };
            var response = Downloader.Get(getRequest);
            if (response.StatusCode == 429)
                return new() { is_too_many_requests = true, error = "Слишком много запросов. Попробуйте через 10-15 минут, но не пытайтесь обойти эту блокировку, иначе она будет автоматически продлеваться." };
            else if (!response.Success)
                return new() { error = response.Data ?? response.ErrorMessage ?? "Нет возвратных данных и данных об ошибке" };
            else if (string.IsNullOrEmpty(response.Data))
                return new() { error = $"Пустая data. Статус код: {response.StatusCode} ({response.EResult})" };
            var inv = JsonSerializer.Deserialize<SteamInventory>(GetDataReplaced(response.Data));
            inv.context = inv.GetContext(appid, context);
            return inv;
        }
        catch (Exception e)
        { return new() { error = e.Message}; }
        
    }
    public async static Task<SteamInventory> LoadAsync(ISessionProvider? session, System.Net.IWebProxy? proxy, ulong steamid64, uint appid, string context = "2", bool trading = false)
    {
        string url = GetUrl(steamid64, appid, context, trading);
        try
        {
            var getRequest = new GetRequest(url, proxy, session) { IsAjax = true };
            var response = await Downloader.GetAsync(getRequest);
            if (response.StatusCode == 429)
                return new() { is_too_many_requests = true, error = "Слишком много запросов. Попробуйте через 10-15 минут, но не пытайтесь обойти эту блокировку, иначе она будет автоматически продлеваться." };
            else if (!response.Success)
                return new() { error = response.Data ?? response.ErrorMessage ?? response.ErrorException?.Message ?? "Нет возвратных данных и данных об ошибке" };
            else if (string.IsNullOrEmpty(response.Data))
                return new() { error = $"Пустая data. Статус код: {response.StatusCode} ({response.EResult})" };
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(GetDataReplaced(response.Data)));
            var inv = await JsonSerializer.DeserializeAsync<SteamInventory>(stream);
            inv.context = inv.GetContext(appid, context);
            return inv;
        }
        catch (Exception e)
        { return new() { error = e.Message }; }
    }
    /// <summary>
    /// Загружает указанный инвентарь указанного пользователя
    /// </summary>
    /// <param name="steamid64"></param>
    /// <param name="appid"></param>
    /// <param name="Session"></param>
    /// <param name="Proxy"></param>
    /// <param name="context">Применяется, если appid = 753</param>
    /// <returns></returns>
    public async static Task<SteamInventory> LoadAsync(ISessionProvider? session, Proxy? proxy, string steamid64, string appid, string context = "2")
     => await LoadAsync(session, proxy, steamid64.ParseUInt64(), appid.ParseUInt32(), context);
    public static SteamInventory Restore(string steamid64, string appid, string? dir = null)
    {
        if (dir == null)
            dir = Path.Join(Environment.CurrentDirectory, $"{appid}");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        string path = Path.Join(dir, $"{steamid64}.json");
        if (File.Exists(path))
        {
            try
            {
                string data = File.ReadAllText(path);
                var options = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                var obj = JsonSerializer.Deserialize<SteamInventory>(data, options);
                return obj;
            }
            catch (Exception e)
            { return new() { error = e.Message }; }
        }
        return new() { error = $"Файла '{path}' не существует" };
    }
    public bool Save(string steamid64, string appid, string? dir = null)
    {
        if (dir == null)
            dir = Path.Join(Environment.CurrentDirectory, $"{appid}");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        string path = Path.Join(dir, $"{steamid64}.json");
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var data = JsonSerializer.Serialize(this, options);
        try
        {
            File.WriteAllText(path, data);
            return true;
        }
        catch (Exception ex)
        { }
        return false;
    }
    public bool Save(ulong steamid64, uint appid, string? dir = null)
    {
        if (dir == null)
            dir = Path.Join(Environment.CurrentDirectory, $"{appid}");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        string path = Path.Join(dir, $"{steamid64}.json");
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var data = JsonSerializer.Serialize(this, options);
        try
        {
            File.WriteAllText(path, data);
            return true;
        }
        catch (Exception ex)
        { }
        return false;
    }

    public Description? GetDescription(string classid, string instanceid)
    {
        var key = $"{classid}_{instanceid}";
        if (rgDescriptions.ContainsKey(key)) return rgDescriptions[key];
        return null;
    }
    public Asset? GetAsset(string classid, string instanceid)
    {
        var assets = GetAssets();
        for (int i = 0; i < assets.Length; i++)
        {
            var item = assets[i];
            if (item.classid == classid &&
                item.instanceid == instanceid)
                return item;
        }
        return null;
    }
    public Asset? GetAsset(string id)
    {
        if (rgInventory.ContainsKey(id))
            return rgInventory[id];
        return null;
    }
    public Asset[] GetAssets()
    {
        var list = new Asset[rgInventory.Count];
        rgInventory.Values.CopyTo(list, 0);
        return list;
    }
    public Description[] GetDescriptions()
    {
        var list = new Description[rgDescriptions.Count];
        rgDescriptions.Values.CopyTo(list, 0);
        return list;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="context">Обычный context 753=6, но также может быть и другой, например 7</param>
    /// <returns>2 - если приложение не найдено (вдруг заработает)</returns>
    public string GetContext(uint appId, string context = "6")
    {
        if (appId == 753) return context;
        return "2";
    }
    public string GetContext(string appId, string context = "6")
    {
        var digit = appId.GetOnlyDigit();
        if (digit == "") return GetContext(0, context);
        return GetContext(uint.Parse(digit), context);
    }


    private static string GetUrl(ulong steamid64, uint appid, string context, bool trading)
    {
        string url = $"https://steamcommunity.com/profiles/{steamid64}/inventory/json";
        if (appid == 440) url += "/440/2/?l=english";
        else if (appid == 730) url += "/730/2/?l=english";
        else if (appid == 753) url += $"/753/{context}/?l=english";
        else if (appid == 218620) url += "/218620/2/?l=english";
        else if (appid == 252490) url += "/252490/2/?l=english";
        else if (appid == 570) url += "/570/2/?l=english";
        else if (appid == 433850) url += "/433850/1/?l=english";
        else url = $"{url}/{appid}/{context}/?l=english";
        if (trading) url += "&trading=1";
        return url;
    }
    private static string GetDataReplaced(string data) => data.Replace(",\"descriptions\":\"\"", "")
                .Replace(",\"owner_descriptions\":\"\"", "")
                .Replace(",\"actions\":\"\"", "")
                .Replace(",\"tags\":\"\"", "")
                .Replace(",\"market_actions\":\"\"", "")
                .Replace("\"rgInventory\":[]", "\"rgInventory\":{}")
                .Replace("\"rgCurrency\":[]", "\"rgCurrency\":{}")
                .Replace("\"rgDescriptions\":[]", "\"rgDescriptions\":{}");
}
