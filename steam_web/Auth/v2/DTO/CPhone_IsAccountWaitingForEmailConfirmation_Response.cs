using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class CPhone_IsAccountWaitingForEmailConfirmation_Response
{
    /// <summary>
    /// Ожидать ли подтверждения почты?
    /// </summary>
    [ProtoMember(1)] public bool awaiting_email_confirmation { get; set; }
    /// <summary>
    /// Через сколько делать следующий запрос в сек
    /// </summary>
    [ProtoMember(2)] public int seconds_to_wait { get; set; }
}
