using System.Text.Json;
using SteamWeb.Extensions;

namespace SteamWeb.Models;
public class SteamTradeError
{
    public string? strError { get; init; }
    public int codeError { get; private set; }

    internal static int GetCode(string strError)
    {
        var between = strError.GetBetween("(", ")");
        return int.TryParse(between, out int result) ? result : 0;
    }
    public static SteamTradeError Deserialize(string json)
    {
        if (json.IsEmpty() || json == "null")
            return new() { strError = "Пустые данные json." };
        try
        {
            var steamerror = JsonSerializer.Deserialize<SteamTradeError>(json);
            steamerror!.codeError = GetCode(steamerror.strError!);
            return steamerror;
        }
        catch (Exception)
        {
            return new() { strError = json };
        }
    }
}
