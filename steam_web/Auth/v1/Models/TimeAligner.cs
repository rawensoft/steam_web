using System.Net;
using System.Text.Json;
using SteamWeb.Extensions;

namespace SteamWeb.Auth.v1;
public partial class TimeAligner
{
    private static bool aligned = false;
    private static int timeDifference = 0;

    public static int GetSteamTime()
    {
        if (!aligned) AlignTime();
        return Util.GetSystemUnixTime() + timeDifference;
    }
    public static async Task<long> GetSteamTimeAsync()
    {
        if (!aligned) await AlignTimeAsync();
        return Util.GetSystemUnixTime() + timeDifference;
    }

    public static void AlignTime()
    {
        int currentTime = Util.GetSystemUnixTime();
        using var client = new WebClient();
        try
        {
            string response = client.UploadString(APIEndpoints.TWO_FACTOR_TIME_QUERY, "steamid=0");
            TimeQuery query = JsonSerializer.Deserialize<TimeQuery>(response);
            timeDifference = query.response.server_time.ParseInt32() - currentTime;
            aligned = true;
        }
        catch (WebException)
        { }
    }
    public static async Task AlignTimeAsync()
    {
        int currentTime = Util.GetSystemUnixTime();
        using var client = new WebClient();
        try
        {
            string response = await client.UploadStringTaskAsync(APIEndpoints.TWO_FACTOR_TIME_QUERY, "steamid=0");
            TimeQuery query = JsonSerializer.Deserialize<TimeQuery>(response);
            timeDifference = query.response.server_time.ParseInt32() - currentTime;
            aligned = true;
        }
        catch (WebException)
        { }
    }
}
