using System.Text.Json;
using System.Text.Json.Serialization;
using SteamWeb.Extensions;

namespace SteamWeb.Models;
public class SteamTradeError
{
    [JsonPropertyName("strError")] public string? StrError { get; init; }
	[JsonPropertyName("codeError")] public int CodeError { get; private set; }

    internal static int GetCode(string strError)
    {
        var between = strError.GetBetween("(", ")");
        return int.TryParse(between, out int result) ? result : 0;
    }
    public static SteamTradeError Deserialize(string json)
    {
        if (json.IsEmpty() || json == "null")
            return new() { StrError = "Пустые данные json." };
        try
        {
            var steamerror = JsonSerializer.Deserialize<SteamTradeError>(json);
            steamerror!.CodeError = GetCode(steamerror.StrError!);
            return steamerror;
        }
        catch (Exception)
        {
            return new() { StrError = json };
        }
    }
}