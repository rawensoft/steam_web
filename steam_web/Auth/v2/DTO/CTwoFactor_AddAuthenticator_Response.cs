using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class CTwoFactor_AddAuthenticator_Response
{
    /// <summary>
    /// Shared secret between server and authenticator
    /// </summary>
    [ProtoMember(1)] public byte[] shared_secret { get; set; }
    /// <summary>
    /// Authenticator serial number (unique per token)
    /// </summary>
    [ProtoMember(2, DataFormat = DataFormat.FixedSize)] public ulong serial_number { get; set; }
    [ProtoMember(3)] public string revocation_code { get; set; }
    /// <summary>
    /// URI for QR code generation
    /// </summary>
    [ProtoMember(4)] public string uri { get; set; }
    [ProtoMember(5)] public int server_time { get; set; }
    [ProtoMember(6)] public string account_name { get; set; }
    /// <summary>
    /// Token GID assigned by server
    /// </summary>
    [ProtoMember(7)] public string token_gid { get; set; }
    /// <summary>
    /// Secret used for identity attestation (e.g., for eventing)
    /// </summary>
    [ProtoMember(8)] public byte[] identity_secret { get; set; }
    /// <summary>
    /// Spare shared secret
    /// </summary>
    [ProtoMember(9)] public byte[] secret_1 { get; set; }
	/// <summary>
	/// Основывается на x-eresult заголовке
	/// <para/>
	/// 1 - ожидание ввода кода с почты или из телефона
	/// <para/>
	/// 2 - нужно привязать номер
	/// <para/>
	/// 11 - EResult.InvalidState
	/// </summary>
	[ProtoMember(10)] public int status { get; set; }
    /// <summary>
    /// a portion of the phone number the SMS code was sent to
    /// </summary>
    [ProtoMember(11)] public string? phone_number_hint { get; set; }
	/// <summary>
	/// how we expect to confirm adding the authenticator
	/// <para/>
	/// 3 - ожидание ввода кода с почты
	/// <para/>
	/// ? - неизвестно
	/// </summary>
	[ProtoMember(12)] public int confirm_type { get; set; }
}
