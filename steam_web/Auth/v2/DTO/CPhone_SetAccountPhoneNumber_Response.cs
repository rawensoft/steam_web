using ProtoBuf;

namespace SteamWeb.Auth.v2.Models;

[ProtoContract]
class CPhone_SetAccountPhoneNumber_Response
{
    [ProtoMember(1)] public string confirmation_email_address { get; set; }
    [ProtoMember(2)] public string phone_number_formatted { get; set; }
}
