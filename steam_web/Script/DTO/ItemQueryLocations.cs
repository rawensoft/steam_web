using System.Text.Json.Serialization;

namespace SteamWeb.Script.DTO;

public class ItemQueryLocations
{
    public string countrycode { get; init; }
    public string countryname { get; internal set; }
    public string statecode { get; init; }
    public string statename { get; internal set; }
    public int cityid { get; init; } = 0;
    public string cityname { get; init; }
    public int hasstates { get; internal set; } = 0;
    public bool state_loaded { get; internal set; } = false;
    public bool city_loaded { get; internal set; } = false;

    [JsonIgnore]
    public bool has_states => hasstates == 1;
}
