using System.Text.Json;
using SteamWeb.Inventory.V2.Models;
using SteamWeb.Web;
using System.Text;
using SteamWeb.Models;
using System.Text.Json.Serialization;

namespace SteamWeb.Inventory.V2;
public class SteamInventory
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = false,
        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
    };

    [JsonPropertyName("success")] public bool Success { get; init; } = false;
    /// <summary>
    /// Key = classid_instanceid
    /// </summary>
    [JsonPropertyName("rgInventory")] public Dictionary<string, Asset> RgInventory { get; init; } = new(1);
    /// <summary>
    /// Key = id
    /// </summary>
    [JsonPropertyName("rgDescriptions")] public Dictionary<string, Description> RgDescriptions { get; init; } = new(1);
    [JsonPropertyName("more")] public bool More { get; init; } = false;
    [JsonPropertyName("context")] public byte Context { get; set; } = 2;
    [JsonPropertyName("error")] public string? Error { get; init; }
    [JsonPropertyName("is_too_many_requests")] public bool IsTooManyRequests { get; init; } = false;

    public static SteamInventory Load(DefaultRequest defaultRequest, ulong steamid64, uint appid, byte context = 2, bool trading = false)
    {
        string url = GetUrl(steamid64, appid, context, trading);
        var getRequest = new GetRequest(url)
        {
            Proxy = defaultRequest.Proxy,
            Session = defaultRequest.Session,
            CancellationToken = defaultRequest.CancellationToken,
            IsAjax = true,
        };
        var response = Downloader.Get(getRequest);
        if (response.StatusCode == 429)
            return new() { IsTooManyRequests = true, Error = "Слишком много запросов. Попробуйте через 10-15 минут, но не пытайтесь обойти эту блокировку, иначе она будет автоматически продлеваться." };
        else if (!response.Success)
            return new() { Error = response.Data ?? response.ErrorMessage ?? "Нет возвратных данных и данных об ошибке" };
        else if (string.IsNullOrEmpty(response.Data))
            return new() { Error = $"Пустая data. Статус код: {response.StatusCode} ({response.EResult})" };

        try
        {
            var inv = JsonSerializer.Deserialize<SteamInventory>(GetDataReplaced(response.Data), _jsonOptions)!;
            inv.Context = context;
            return inv;
        }
        catch (Exception e)
        {
            return new()
            {
                Error = e.Message
            };
        }
    }
    public async static Task<SteamInventory> LoadAsync(DefaultRequest defaultRequest, ulong steamid64, uint appid, byte context = 2, bool trading = false)
    {
        string url = GetUrl(steamid64, appid, context, trading);
        var getRequest = new GetRequest(url)
        {
            Proxy = defaultRequest.Proxy,
            Session = defaultRequest.Session,
            CancellationToken = defaultRequest.CancellationToken,
            IsAjax = true,
        };
        var response = await Downloader.GetAsync(getRequest);
        if (response.StatusCode == 429)
            return new() { IsTooManyRequests = true, Error = "Слишком много запросов. Попробуйте через 10-15 минут, но не пытайтесь обойти эту блокировку, иначе она будет автоматически продлеваться." };
        else if (!response.Success)
            return new() { Error = response.Data ?? response.ErrorMessage ?? response.ErrorException?.Message ?? "Нет возвратных данных и данных об ошибке" };
        else if (string.IsNullOrEmpty(response.Data))
            return new() { Error = $"Пустая data. Статус код: {response.StatusCode} ({response.EResult})" };

        try
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(GetDataReplaced(response.Data)));
            var inv = await JsonSerializer.DeserializeAsync<SteamInventory>(stream, _jsonOptions)!;
            inv!.Context = context;
            return inv;
        }
        catch (Exception e)
        {
            return new()
            {
                Error = e.Message
            };
        }
    }

    public static SteamInventory Restore(string steamid64, uint appid, string? dir = null)
    {
        if (dir == null)
            dir = Path.Join(Environment.CurrentDirectory, appid.ToString());

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string path = Path.Join(dir, steamid64 + ".json");
        if (File.Exists(path))
        {
            try
            {
                string data = File.ReadAllText(path);
                var options = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString,
                };
                var obj = JsonSerializer.Deserialize<SteamInventory>(data, options)!;
                return obj;
            }
            catch (Exception e)
            {
                return new()
                {
                    Error = e.Message
                };
            }
        }
        return new() { Error = $"Файла '{path}' не существует" };
    }
    public bool Save(ulong steamid64, uint appid, string? dir = null)
    {
        if (dir == null)
            dir = Path.Join(Environment.CurrentDirectory, appid.ToString());

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string path = Path.Join(dir, steamid64 + ".json");
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        var data = JsonSerializer.Serialize(this, options);
        try
        {
            File.WriteAllText(path, data);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Description? GetDescription(string classid, string instanceid)
    {
        var key = classid + "_" + instanceid;
        if (RgDescriptions.TryGetValue(key, out var value))
            return value;
        return null;
    }
    public Asset? GetAsset(string classid, string instanceid)
    {
        foreach (var (_, asset) in RgInventory)
        {
            if (asset.ClassId == classid &&
                asset.InstanceId == instanceid)
                return asset;
        }
        return null;
    }
    public Asset? GetAsset(string id)
    {
        if (RgInventory.TryGetValue(id, out var value))
            return value;
        return null;
    }
    public Asset[] GetAssets()
    {
        var array = RgInventory.Values.ToArray();
        return array;
    }
    public Description[] GetDescriptions()
    {
        var array = RgDescriptions.Values.ToArray();
        return array;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="context">Обычный context 753=6, но также может быть и другой, например 7</param>
    /// <returns>2 - если приложение не найдено (вдруг заработает)</returns>
    public byte GetContext(uint appId, byte context = 6)
    {
        if (appId == 753)
            return context;
        return 2;
    }

    private static string GetUrl(ulong steamid64, uint appid, byte context, bool trading)
    {
        var sb = new StringBuilder(10);
        sb.Append("https://steamcommunity.com/profiles/");
        sb.Append(steamid64);
        sb.Append("/inventory/json");

        if (appid == 730)
            sb.Append("/730/2/?l=english");
        else if (appid == 753)
        {
            sb.Append("/753/");
            sb.Append(context);
            sb.Append("/?l=english");
        }
        else if(appid == 440)
            sb.Append("/440/2/?l=english");
        else if (appid == 570)
            sb.Append("/570/2/?l=english");
        else if (appid == 252490)
            sb.Append("/252490/2/?l=english");
        else if (appid == 218620)
            sb.Append("/218620/2/?l=english");
        else if (appid == 433850)
            sb.Append("/433850/1/?l=english");
        else
        {
            sb.Append('/');
            sb.Append(appid);
            sb.Append('/');
            sb.Append(context);
            sb.Append("/?l=english");
        }

        if (trading)
            sb.Append("&trading=1");
        return sb.ToString();
    }
    private static string GetDataReplaced(string data) => data.Replace(",\"descriptions\":\"\"", string.Empty)
                .Replace(",\"owner_descriptions\":\"\"", string.Empty)
                .Replace(",\"actions\":\"\"", string.Empty)
                .Replace(",\"tags\":\"\"", string.Empty)
                .Replace(",\"market_actions\":\"\"", string.Empty)
                .Replace("\"rgInventory\":[]", "\"rgInventory\":{}")
                .Replace("\"rgCurrency\":[]", "\"rgCurrency\":{}")
                .Replace("\"rgDescriptions\":[]", "\"rgDescriptions\":{}");
}