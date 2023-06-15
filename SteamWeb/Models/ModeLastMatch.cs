using System.Text.Json.Serialization;
namespace SteamWeb.Models;
public class ModeLastMatch
{
    public string Mode { get; init; }
    public string LastMatch { get; init; }

    [JsonConstructor]
    public ModeLastMatch(string Mode, string LastMatch)
    {
        this.Mode = Mode;
        this.LastMatch = LastMatch;
    }
}
