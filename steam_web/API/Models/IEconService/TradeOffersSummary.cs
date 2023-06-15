using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IEconService
{
    public class TradeOffersSummary
    {
        public uint pending_received_count { get; set; }
        public uint new_received_count { get; set; }
        public uint updated_received_count { get; set; }
        public uint historical_received_count { get; set; }
        public uint pending_sent_count { get; set; }
        public uint newly_accepted_sent_count { get; set; }
        public uint updated_sent_count { get; set; }
        public uint historical_sent_count { get; set; }
        public uint escrow_received_count { get; set; }
        public uint escrow_sent_count { get; set; }
    }
}
