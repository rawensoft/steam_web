using ProtoBuf;
using System.Text.Json.Serialization;

namespace SteamWeb.API.Protobufs;
[ProtoContract]
public class CMobileAppService_GetMobileSummary_Request
{
    /// <summary>
    /// GID of the mobile app's authenticator for this user
    /// <para/>
    /// Convert <see cref="Auth.v2.SteamGuardAccount.TokenGID"/> from HEX to ulong:
    /// <code>
    /// var bytes = Convert.FromHexString(TokenGID);
    /// var token_gid = BitConverter.ToUInt64(bytes);
    /// </code>
    /// </summary>
    [ProtoMember(1, DataFormat = DataFormat.FixedSize)]
    [JsonPropertyName("authenticator_gid")]
    public ulong AuthenticatorGID { get; init; }
}
