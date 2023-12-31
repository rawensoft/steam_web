﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IEconService
{
    public class TradesHistory: TradeStatus
    {
        /// <summary>
        /// Есть значение если include_total == true
        /// </summary>
        public uint? total_trades { get; set; } = null;
        public bool more { get; set; } = false;
        
        public List<TradeHistory> GetTradesForUser(ulong steamid64_other)
        {
            var list = new List<TradeHistory>();
            foreach (var item in trades)
            {
                if (item.u_steamid_other == steamid64_other) list.Add(item);
            }
            return list;
        }
        public List<TradeHistory> GetTradesForUser(string steamid64_other)
        {
            var list = new List<TradeHistory>();
            foreach (var item in trades)
            {
                if(item.steamid_other == steamid64_other) list.Add(item);
            }
            return list;
        }
    }
}
