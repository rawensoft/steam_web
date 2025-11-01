using System.Text.Json;
using SteamWeb.Inventory.V1.Models;
using SteamWeb.Web;
using SteamWeb.Auth.Interfaces;
using SteamWeb.Extensions;

namespace SteamWeb.Inventory.V1;
public class SteamInventory
{
    public Asset[] assets { get; set; } = Array.Empty<Asset>();
    public Description[] descriptions { get; set; } = Array.Empty<Description>();
    public string? last_assetid { get; set; }
    public uint? more_items { get; set; }
    public int? rwgrsn { get; set; }
    public int? success { get; set; }
    public uint? total_inventory_count { get; set; }

    public async static Task<SteamInventory> LoadAsync(ISessionProvider? session, System.Net.IWebProxy? proxy, ulong steamid64, uint appid, ushort count, string contextid)
    {
        string url = $"http://steamcommunity.com/inventory/{steamid64}/{appid}/{contextid}?l=english&count={count}";
        try
        {
            var getRequest = new GetRequest(url, proxy, session) { IsAjax = true };
            var response = await Downloader.GetAsync(getRequest);
            if (!response.Success)
                return new SteamInventory() { success = 0 };
            else if (string.IsNullOrEmpty(response.Data))
                return new SteamInventory() { success = 0 };
            var inv = JsonSerializer.Deserialize<SteamInventory>(response.Data)!;
            return inv;
        }
        catch (Exception)
        {
			return new SteamInventory() { success = 0 };
		}
    }
    public static SteamInventory Restore(string steamid64, string appid, string? dir = null)
    {
        if (dir == null)
            dir = Environment.CurrentDirectory + $"\\{appid}\\";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        string path = dir + steamid64 + ".json";
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
                var obj = JsonSerializer.Deserialize<SteamInventory>(data, options)!;
                return obj;
            }
            catch (Exception)
            {
				return new SteamInventory() { success = -1 };
			}
        }
        return new SteamInventory() { success = 0 };
    }
    public void Save(string steamid64, string appid, string? dir = null)
    {
        if (dir == null)
            dir = Environment.CurrentDirectory + $"\\{appid}\\";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        string path = dir + steamid64 + ".json";
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
        }
        catch (Exception)
        {

        }
	}
	public Description? GetDescription(string classid, string instanceid)
	{
        var classidN = classid.ParseUInt64();
		var instanceidN = instanceid.ParseUInt64();
		for (int i = 0; i < descriptions.Length; i++)
			if (descriptions[i].ClassId == classidN &&
				descriptions[i].InstanceId == instanceidN)
				return descriptions[i];
		return null;
	}
	public Description? GetDescription(ulong classid, ulong instanceid)
    {
        for (int i = 0; i < descriptions.Length; i++)
			if (descriptions[i].ClassId == classid &&
				descriptions[i].InstanceId == instanceid)
				return descriptions[i];
		return null;
    }
}