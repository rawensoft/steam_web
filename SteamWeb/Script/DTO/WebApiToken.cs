using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteamWeb.Script.DTO
{
    public class WebApiToken
    {
        [JsonPropertyName("webapi_token")] public string WebAPI_Token { get; set; }
    }
}
