using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace SteamWeb.Models;
public record InformationProfileRequest
{
    private readonly SessionData _session;
    public string weblink_1_title { get; init; } = "";
    public string weblink_1_url { get; init; } = "";
    public string weblink_2_title { get; init; } = "";
    public string weblink_2_url { get; init; } = "";
    public string weblink_3_title { get; init; } = "";
    public string weblink_3_url { get; init; } = "";
    public string personaName { get; init; } = "";
    public string real_name { get; init; } = "";
    public string customURL { get; init; } = "";
    public string country { get; init; } = "";
    public int state { get; init; } = 0;
    public int city { get; init; } = 0;
    public string summary { get; init; } = "No information given.";
    public int hide_profile_awards { get; init; } = 0;
    [JsonIgnore] public string sessionID => _session.SessionID;
    [JsonIgnore] public string steamID => _session.SteamID.ToString();
    public int json { get; init; } = 1;

    public InformationProfileRequest(SessionData Session) => _session = Session;
    public KeyValuePair<string, string>[] GetPostData(string type = "profileSave")
    {
        var content = new KeyValuePair<string, string>[]
        {
            new("sessionID", sessionID),
            new("weblink_1_title", weblink_1_title),
            new("weblink_1_url", weblink_1_url),
            new("weblink_2_title", weblink_2_title),
            new("weblink_2_url", weblink_2_url),
            new("weblink_3_title", weblink_3_title),
            new("weblink_3_url", weblink_3_url),
            new("personaName", personaName),
            new("real_name", real_name),
            new("customURL", customURL),
            new("country", country),
            new("state", state.ToString()),
            new("city", city.ToString()),
            new("summary", summary),
            new("hide_profile_awards", hide_profile_awards.ToString()),
            new("type", type),
            new("json", json.ToString()),
        };
        return content;
    }
}
