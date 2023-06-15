using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class AuthPollResponse
{
    [ProtoMember(1)] public string new_client_id { get; set; }
    [ProtoMember(2)] public string new_challenge_url { get; set; }
    [ProtoMember(3)] public string refresh_token { get; set; }
    /// <summary>
    /// steamLoginSecure={steamid64}%7C%7C{access_token}
    /// </summary>
    [ProtoMember(4)] public string access_token { get; set; }
    /// <summary>
    /// whether or not the auth session appears to have had remote interaction from a potential confirmer
    /// </summary>
    [ProtoMember(5)] public bool had_remote_interaction { get; set; } = false;
    [ProtoMember(6)] public string account_name { get; set; }
}
