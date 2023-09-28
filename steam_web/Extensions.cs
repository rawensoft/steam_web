using System.Text;

namespace SteamWeb.Extensions;
public static class ExtensionMethods
{
    private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

	public static Dictionary<T1, T2> SubDict<T1, T2>(this Dictionary<T1, T2> data, int index, int length)
    {
        var result = new Dictionary<T1, T2>(length + 1);

        var keys = data.Keys.ToArray();
        T1[] result1 = new T1[length];
        Array.Copy(keys, index, result1, 0, length);

        var values = data.Values.ToArray();
        T2[] result2 = new T2[length];
        Array.Copy(values, index, result2, 0, length);

        for (int i = 0; i < length; i++)
            result.Add(result1[i], result2[i]);

        return result;
    }
    public static T[] SubArray<T>(this T[] data, int index, int length)
    {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }
    public static short ParseInt16(this string value)
    {
        if (short.TryParse(value, out short result)) return result;
        return 0;
    }
    public static int ParseInt32(this string value)
    {
        if (int.TryParse(value, out int result)) return result;
        return 0;
    }
    public static long ParseInt64(this string value)
    {
        if (long.TryParse(value, out long result)) return result;
        return 0;
    }
    public static ushort ParseUInt16(this string value)
    {
        if (ushort.TryParse(value, out ushort result)) return result;
        return 0;
    }
    public static uint ParseUInt32(this string value)
    {
        if (uint.TryParse(value, out uint result)) return result;
        return 0;
    }
    public static ulong ParseUInt64(this string value)
    {
        if (ulong.TryParse(value, out ulong result)) return result;
        return 0;
    }
    public static double ParseDouble(this string value)
    {
        value = value.Replace(".", ",");
        if (double.TryParse(value, out double result)) return result;
        return 0;
    }
    public static float ParseFloat(this string value)
    {
        value = value.Replace(".", ",");
        if (float.TryParse(value, out float result)) return result;
        return 0f;
    }
    public static string? GetClearWebString(this string? strSource) => strSource?.Replace("\n", "").Replace("\t", "").Replace("\r", "");
    public static bool IsEmpty(this string? str) => string.IsNullOrEmpty(str);
    public static bool ContainsOnlyDigit(this string strSource)
    {
        for (int i = 0; i < strSource.Length; i++)
        {
            if (!Char.IsDigit(strSource[i]))
                return false;
        }
        return true;
    }
    public static string GetOnlyDigit(this string strSource)
    {
        if (string.IsNullOrEmpty(strSource))
            return "";
        var sb = new StringBuilder();
        for (int i = 0; i < strSource.Length; i++)
        {
            if (Char.IsDigit(strSource[i]))
                sb.Append(strSource[i]);
        }
        return sb.ToString();
    }
    public static string Formatted(this DateTime dt) => dt.ToString("dd.MM.yyyy HH:mm:ss");
    public static int ToTimeStamp(this DateTime dt) => (int)(dt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    public static int ToUnixTimeStamp(this DateTime dt) => ToTimeStamp(dt);
    public static DateTime ToDateTime(this int seconds)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var timeSpan = TimeSpan.FromSeconds(seconds);
        epoch = epoch.Add(timeSpan);
        return epoch.ToLocalTime();
    }
    public static int ToInt32(this bool value) => value ? 1 : 0;
    public static string? GetBetween(this string strSource, string strStart, string strEnd, string? replace = null, int x = 0)
    {
        x += 1;
        if (strSource.IsEmpty())
            return replace;
        bool is_empty_start = strStart.IsEmpty();
        if (is_empty_start || (!is_empty_start! && strSource.Contains(strStart) && strSource.Contains(strEnd)))
        {
            if (is_empty_start)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < strSource.Length; i++)
                {
                    if (char.IsDigit(strSource[i]))
                        break;
                    sb.Append(strSource[i]);
                }
                strStart = sb.ToString();
            } // если начало пустое, значит нужно найти первый digit символ
            if (strStart == "")
                return replace;

            string[] splitted = strSource.Split(strStart);
            if (splitted.Length <= x) return replace;
            return splitted[x].Split(strEnd)[0];
        }
        else return replace;
    }
    public static bool RemoveAsset(this Inventory.V2.SteamInventory inventory, Inventory.V2.Models.Asset asset)
    {
        return inventory.rgInventory.Remove($"{asset.classid}_{asset.instanceid}");
    }
    public static bool RemoveAsset(this Inventory.V2.SteamInventory inventory, string classid, string instanceid)
    {
        return inventory.rgInventory.Remove($"{classid}_{instanceid}");
    }
	public static string GetRandomString(this int length)
	{
		var random = new Random();
		var sb = new StringBuilder(length);
		var lengthChars = _chars.Length;
		for (int i = 0; i < length; i++)
			sb.Append(_chars[random.Next(lengthChars)]);
		return sb.ToString();
	}
}
