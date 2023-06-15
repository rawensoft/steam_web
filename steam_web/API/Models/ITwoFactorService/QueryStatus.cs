using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.ITwoFactorService
{
    public class QueryStatus
    {
        public uint state {  get; set; }
        public uint inactivation_reason { get; set; }
        public uint authenticator_type { get; set; }
        public bool authenticator_allowed { get; set; } = false;
        public uint steamguard_scheme { get; set; }
        public string token_gid { get; set; }
        public bool email_validated { get; set; } = false;
        public string device_identifier { get; set; }
        public int time_created { get; set; }
        public ushort revocation_attempts_remaining { get; set; }
        public string classified_agent { get; set; }
        public bool allow_external_authenticator { get; set; } = false;
    }
}
