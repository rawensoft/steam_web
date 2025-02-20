using ProtoBuf;
using System.Text.Json.Serialization;

namespace SteamWeb.API.Protobufs;
[ProtoContract]
public class CMobileAppService_GetMobileSummary_Request
{
    /// <summary>
    /// GID of the mobile app's authenticator for this user
    /// </summary>
    [ProtoMember(1, DataFormat = DataFormat.FixedSize)]
    [JsonPropertyName("authenticator_gid")]
    public ulong AuthenticatorGID { get; init; }
}
