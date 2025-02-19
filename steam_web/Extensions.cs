using SteamWeb.Auth.v2.Enums;
using SteamWeb.Auth.v2.Models;
using SteamWeb.Enums;
using SteamWeb.Models;
using SteamWeb.Script.Enums;
using SteamWeb.Script.Models;
using SteamWeb.Web;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SteamWeb.Extensions;
public static class ExtensionMethods
{
    private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
	private const string _dtFormat = "dd.MM.yyyy HH:mm:ss";
    private static readonly Regex _rgxMarketTrans = new(@"^(\d{1,7}) Market Transactions$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
    private static readonly Regex _rgxSteamId = new(@"^7656119([0-9]{10})$", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromMilliseconds(100));
    private static readonly Regex _rgxSteamId2 = new(@"^STEAM_0:[0-1]:([0-9]{1,10})$", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromMilliseconds(100));
    private static readonly Regex _rgxSteamId3 = new(@"^U:[0-1]:([0-9]{1,10})$", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromMilliseconds(100));

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
	public static byte ParseByte(this string? value)
	{
		if (byte.TryParse(value, out byte result))
			return result;
		return 0;
	}
	public static short ParseInt16(this string? value)
    {
        if (short.TryParse(value, out short result))
            return result;
        return 0;
    }
    public static int ParseInt32(this string? value)
    {
        if (int.TryParse(value, out int result))
            return result;
        return 0;
    }
    public static long ParseInt64(this string? value)
    {
        if (long.TryParse(value, out long result))
            return result;
        return 0;
    }
    public static ushort ParseUInt16(this string? value)
    {
        if (ushort.TryParse(value, out ushort result))
            return result;
        return 0;
    }
    public static uint ParseUInt32(this string? value)
    {
        if (uint.TryParse(value, out uint result))
            return result;
        return 0;
    }
    public static ulong ParseUInt64(this string? value)
    {
        if (ulong.TryParse(value, out ulong result))
            return result;
        return 0;
	}
	public static decimal ParseDecimal(this string? value)
	{
		if (value == null)
			return 0m;
		value = value.Replace(".", ",");
		if (decimal.TryParse(value, out decimal result))
			return result;
		return 0m;
	}
	public static double ParseDouble(this string? value)
	{
		if (value == null)
			return 0d;
		value = value.Replace(".", ",");
        if (double.TryParse(value, out double result))
            return result;
        return 0;
    }
    public static float ParseFloat(this string? value)
    {
        if (value == null)
            return 0f;
        value = value.Replace('.', ',');
        if (float.TryParse(value, out float result))
            return result;
        return 0f;
    }
    public static string? GetClearWebString(this string? strSource) => strSource?.Replace("\n", "").Replace("\t", "").Replace("\r", "");
    public static bool IsEmpty(this string? str) => string.IsNullOrEmpty(str);
    public static bool ContainsOnlyDigit(this string strSource)
    {
        var length = strSource.Length;
        for (int i = 0; i < length; i++)
        {
            if (!char.IsDigit(strSource[i]))
                return false;
        }
        return true;
    }
    public static string GetOnlyDigit(this string? strSource)
    {
        if (string.IsNullOrEmpty(strSource))
            return string.Empty;
		var length = strSource.Length;
		var sb = new StringBuilder(length + 1);
        for (int i = 0; i < length; i++)
        {
            var ch = strSource[i];
            if (char.IsDigit(ch))
                sb.Append(ch);
        }
        return sb.ToString();
    }
    public static string Formatted(this DateTime dt) => dt.ToString(_dtFormat);
    public static int ToTimeStamp(this DateTime dt)
    {
        var seconds = new DateTimeOffset(dt).ToUnixTimeSeconds();
		return (int)seconds;
	}
    public static int ToUnixTimeStamp(this DateTime dt) => ToTimeStamp(dt);
    /// <summary>
    /// Возвращает локальное время
    /// </summary>
    /// <param name="seconds">unix временная метка в секундах</param>
    /// <returns>Локальное время, на основе unix метки</returns>
    public static DateTime ToDateTime(this int seconds)
    {
        var offset = DateTimeOffset.FromUnixTimeSeconds(seconds);
		return offset.DateTime.ToLocalTime();
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
				var length = strSource.Length;
				var sb = new StringBuilder(length + 1);
                for (int i = 0; i < length; i++)
                {
                    var ch = strSource[i];
                    if (char.IsDigit(ch))
                        break;
                    sb.Append(ch);
                }
                strStart = sb.ToString();
            } // если начало пустое, значит нужно найти первый digit символ
            if (strStart == string.Empty)
                return replace;

            string[] splitted = strSource.Split(strStart);
            if (splitted.Length <= x)
                return replace;
            return splitted[x].Split(strEnd)[0];
        }
        else return replace;
    }
    public static bool RemoveAsset(this Inventory.V2.SteamInventory inventory, Inventory.V2.Models.Asset asset)
    {
        var sb = new StringBuilder(4);
        sb.Append(asset.ClassId);
		sb.Append('_');
		sb.Append(asset.InstanceId);
		return inventory.RgInventory.Remove(sb.ToString());
    }
    public static bool RemoveAsset(this Inventory.V2.SteamInventory inventory, string classid, string instanceid)
	{
		var sb = new StringBuilder(4);
		sb.Append(classid);
		sb.Append('_');
		sb.Append(instanceid);
		return inventory.RgInventory.Remove(sb.ToString());
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
    public static Confirmation? GetByCreatorId(this Confirmation[] confs, ulong creatorid)
    {
        var length = confs.Length;
        for (int i = 0; i < length; i++)
        {
            var conf = confs[i];
            if (conf.creator_id == creatorid)
                return conf;
        }
        return null;
    }
    public static Confirmation? GetByCreatorId(this ConfirmationsResponse confs, ulong creatorid)
    {
        var length = confs.conf.Length;
        for (int i = 0; i < length; i++)
        {
            var conf = confs[i];
            if (conf.creator_id == creatorid)
                return conf;
        }
        return null;
    }

	public static string ToStringValue(this OP_CODES op) => op switch
	{
		OP_CODES.GetSMSCode => "get_sms_code",
		OP_CODES.GetPhoneNumber => "get_phone_number",
		OP_CODES.RetryEmailVerification => "retry_email_verification",
		OP_CODES.ReSendSMS => "resend_sms",
		_ => "email_verification",
	};
	public static int ToDigitMethod(this TypeMethod method) => method switch
	{
		TypeMethod.Mobile => 8,
        TypeMethod.Phone => 4,
        TypeMethod.Email => 2,
		_ => -1,
	};
	public static int ToDigitReset(this TypeReset reset) => reset switch
    {
        TypeReset.Password => 1,
        TypeReset.Email => 2,
		TypeReset.Phone => 4,
        TypeReset.Guard => 8,
        TypeReset.KTEmail => 0,
		TypeReset.KTGuard => 0,
		TypeReset.KTPhone => 0,
		TypeReset.KTPassword => 0,
		_ => -1,
	};
	public static int ToDigitIssueId(this TypeReset reset) => reset switch
	{
		TypeReset.Email => 409,
		TypeReset.Password => 406,
		TypeReset.Phone => 403,
        TypeReset.Guard => 406,
        TypeReset.KTEmail => 0,
        TypeReset.KTPassword => 0,
        TypeReset.KTPhone => 0,
        TypeReset.KTGuard => 0,
		_ => -1,
	};
	public static byte ToDigitLost(this TypeLost lost) => (byte)lost;
    public static PURCHASE_TYPE ToEnumPurchaseType(this string stringValue)
    {
        if (char.IsDigit(stringValue[0]))
        {
            try
            {
                var math = _rgxMarketTrans.Match(stringValue);
                if (math.Success)
                    return PURCHASE_TYPE.MarketTransactions;
            }
            catch (RegexMatchTimeoutException)
            {
                // если всё таки не получилось, тогда идём дальше
            }
        }

        if (stringValue == "Gift Purchase")
            return PURCHASE_TYPE.GiftPurchase;

        if (stringValue == "In-Game Purchase")
            return PURCHASE_TYPE.InGamePurchase;

        if (stringValue == "Purchase")
            return PURCHASE_TYPE.Purchase;

        if (stringValue == "Conversion")
            return PURCHASE_TYPE.Conversion;

        if (stringValue == "Refund")
            return PURCHASE_TYPE.Refund;

        if (stringValue == "Market Transaction")
            return PURCHASE_TYPE.MarketTransaction;

        return PURCHASE_TYPE.Unknown;
    }
    public static ulong ToSteamId64(this uint input) => Steam.SteamIDConverter + input;
    public static uint ToSteamId32(this ulong input) => (uint)(input - Steam.SteamIDConverter);
    /// <summary>
    /// </summary>
    /// <param name="communityId">SteamID64(76561197960265728)</param>
    /// <exception cref="RegexMatchTimeoutException"/>
    /// <returns>String.empty if error, else the string SteamID2(STEAM_0:1:000000)</returns>
    public static string ToSteamId2(this ulong communityId)
    {
        if (communityId < 76561197960265729L || !_rgxSteamId.IsMatch(communityId.ToString(CultureInfo.InvariantCulture)))
            return string.Empty;
        communityId -= Steam.SteamIDConverter;
        ulong num = communityId % 2L;
        communityId -= num;
        string input = string.Format("STEAM_0:{0}:{1}", num, (communityId / 2L));
        if (!_rgxSteamId2.IsMatch(input))
            return string.Empty;
        return input;
    }
    /// <summary>
    /// </summary>
    /// <param name="communityId">SteamID64(76561197960265728)</param>
    /// <exception cref="RegexMatchTimeoutException"/>
    /// <returns>String.empty if error, else the string SteamID3(U:1:000000)</returns>
    public static string ToSteamId3(this ulong communityId)
    {
        if (communityId < 76561197960265729L || !_rgxSteamId.IsMatch(communityId.ToString(CultureInfo.InvariantCulture)))
            return string.Empty;
        communityId -= Steam.SteamIDConverter;
        ulong num = communityId % 2L;
        communityId -= num;
        string input = string.Format("U:{0}:{1}", num, (uint)communityId);
        if (!_rgxSteamId3.IsMatch(input))
            return string.Empty;
        return input;
    }
    /// <summary>
    /// Парсит строку цены в decimal число
    /// </summary>
    /// <param name="priceString">Любая строка цены, включающая символ валюты и сумму</param>
    /// <returns>-1 если не удалось определить, либо число</returns>
    public static decimal ToDecimalPrice(this string? priceString)
    {
        if (priceString.IsEmpty())
            return -1m;

        bool hadLastDigit = false;
        var sb = new StringBuilder(priceString!.Length);
        foreach (char ch in priceString)
        {
            if (char.IsDigit(ch))
            {
                sb.Append(ch);
                hadLastDigit = true;
            }
            else if (hadLastDigit && char.IsPunctuation(ch))
            {
                sb.Append('.');
                hadLastDigit = false;
            }
            else
                hadLastDigit = false;
        }
        if (!decimal.TryParse(sb.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var decimalValue))
            return -1m;

        return decimalValue;
    }
    /// <summary>
    /// Парсит строку цены в uin32 число
    /// </summary>
    /// <param name="priceString">Любая строка цены, включающая символ валюты и сумму</param>
    /// <returns>0 если не удалось определить, либо число</returns>
    public static uint ToUInt32Price(this string? priceString)
    {
        decimal price = priceString.ToDecimalPrice();
        if (price == -1)
            return 0;

        uint value = (uint)Math.Round(price * 100, 0);
        return value;
    }

    public static AjaxWizardRequest CreateWizard(this DefaultRequest request, ulong s, string? referer)
    {
        var wizard = new AjaxWizardRequest
        {
            Session = request.Session,
            Proxy = request.Proxy,
            CancellationToken = request.CancellationToken,
            Referer = referer,
            S = s,
        };
        return wizard;
    }
    public static AjaxInfoRequest CreateInfo(this AjaxWizardRequest request, uint account, TypeLost lost, TypeMethod method, TypeReset reset)
    {
        var wizard = new AjaxInfoRequest
        {
            Session = request.Session,
            Proxy = request.Proxy,
            CancellationToken = request.CancellationToken,
            Referer = request.Referer,
            S = request.S,
            Account = account,
            Lost = lost,
            Method = method,
            Reset = reset
        };
        return wizard;
    }
    public static string FromBase64UrlToBase64(this string base64Url)
    {
        base64Url = base64Url.Replace('_', '/').Replace('-', '+');
        switch (base64Url.Length % 4)
        {
            case 2:
                base64Url += "==";
                break;
            case 3:
                base64Url += "=";
                break;
        }
        return base64Url;
    }

    /// <summary>
    /// Проверяем браузерную сессию на активность
    /// </summary>
    /// <param name="session">Сессия для проверки</param>
    /// <param name="proxy">Прокси для использования в этом запросе</param>
    /// <returns>true если сессия валидная, false если сессия null, AccessToken пустой или PlatformType не WebBrowser, а также в других случаях</returns>
    public static bool IsValid(this SessionData? session, Proxy? proxy = null, CancellationToken? cancellationToken = null)
    {
        if (session == null || session.AccessToken.IsEmpty() || session.PlatformType != EAuthTokenPlatformType.WebBrowser)
            return false;

		var check_session = API.IAuthenticationService.GetAuthSessionsForAccount(session, proxy, cancellationToken);
		if (check_session?.Response.client_ids == null)
            return false;
        return true;
	}
	/// <summary>
	/// Обновляет AccessToken браузерной сессии
	/// </summary>
	/// <param name="session">Сессия для обновления</param>
	/// <param name="proxy">Прокси для использования в этом запросе</param>
	/// <returns>true если сессия обновлена, false если сессия null, RefreshToken пустой или PlatformType не WebBrowser, а также в других случаях</returns>
	public static bool Refresh(this SessionData? session, Proxy? proxy = null, CancellationToken? cancellationToken = null)
	{
		if (session == null || session.RefreshToken.IsEmpty() || session.PlatformType != EAuthTokenPlatformType.WebBrowser)
			return false;

		var new_token_response = API.IAuthenticationService.GenerateAccessTokenForApp(session, proxy, cancellationToken);
        if (new_token_response.Item1 != EResult.OK || new_token_response.Item2?.access_token?.IsEmpty() != false)
            return false;
		else
		{
			session.AccessToken = new_token_response.Item2!.access_token;
            return true;
		}
	}
	/// <summary>
	/// Проверяем браузерную сессию на активность
	/// </summary>
	/// <param name="session">Сессия для проверки</param>
	/// <param name="proxy">Прокси для использования в этом запросе</param>
	/// <returns>true если сессия валидная, false если сессия null, AccessToken пустой или PlatformType не WebBrowser, а также в других случаях</returns>
	public static async Task<bool> IsValidAsync(this SessionData? session, Proxy? proxy = null, CancellationToken? cancellationToken = null)
	{
		if (session == null || session.AccessToken.IsEmpty() || session.PlatformType != EAuthTokenPlatformType.WebBrowser)
			return false;

		var check_session = await API.IAuthenticationService.GetAuthSessionsForAccountAsync(session, proxy, cancellationToken);
		if (check_session?.Response.client_ids == null)
            return false;
        return true;
	}
    /// <summary>
	 /// Обновляет AccessToken браузерной сессии
	 /// </summary>
	 /// <param name="session">Сессия для обновления</param>
	 /// <param name="proxy">Прокси для использования в этом запросе</param>
	 /// <returns>true если сессия обновлена, false если сессия null, RefreshToken пустой или PlatformType не WebBrowser, а также в других случаях</returns>
	public static async Task<bool> RefreshAsync(this SessionData? session, Proxy? proxy = null, CancellationToken? cancellationToken = null)
	{
		if (session == null || session.RefreshToken.IsEmpty() || session.PlatformType != EAuthTokenPlatformType.WebBrowser)
			return false;

		var new_token_response = await API.IAuthenticationService.GenerateAccessTokenForAppAsync(session, proxy, cancellationToken);
		if (new_token_response.Item1 != EResult.OK || new_token_response.Item2?.access_token?.IsEmpty() != false)
			return false;
		else
		{
			session.AccessToken = new_token_response.Item2.access_token;
			return true;
		}
	}
}