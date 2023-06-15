using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IEconService
{
    public record TradeHistoryAssets
    {
        public int appid { get; set; }
        public string context { get; set; }
        public string assetid { get; set; }
        public string amount { get; set; }
        /// <summary>
        /// together with instanceid, uniquely identifies the display of the item
        /// </summary>
        public string classid { get; set; }
        /// <summary>
        /// together with classid, uniquely identifies the display of the item
        /// </summary>
        public string instanceid { get; set; }
        /// <summary>
        /// the asset ID given to the item after the trade completed
        /// </summary>
        public string new_assetid { get; set; }
        /// <summary>
        /// the context ID the item was placed in
        /// </summary>
        public string new_contextid { get; set; }
        /// <summary>
        /// if the trade has been rolled back, the new asset ID given in the rollback
        /// </summary>
        public string rollback_new_assetid { get; set; }
        /// <summary>
        /// if the trade has been rolled back, the context ID the new asset was placed in
        /// </summary>
        public string rollback_new_contextid { get; set; }

        [JsonIgnore] public ushort u_context
        {
            get
            {
                if (ushort.TryParse(context, out var result)) return result;
                return 2;
            }
        }
        [JsonIgnore] public ulong u_assetid
        {
            get
            {
                if (ulong.TryParse(assetid, out var result)) return result;
                return 0;
            }
        }
        [JsonIgnore] public uint u_amount
        {
            get
            {
                if (uint.TryParse(amount, out var result)) return result;
                return 0;
            }
        }
        /// <summary>
        /// together with instanceid, uniquely identifies the display of the item
        /// </summary>
        [JsonIgnore] public ulong u_classid
        {
            get
            {
                if (ulong.TryParse(classid, out var result)) return result;
                return 0;
            }
        }
        /// <summary>
        /// together with classid, uniquely identifies the display of the item
        /// </summary>
        [JsonIgnore] public uint u_instanceid
        {
            get
            {
                if (uint.TryParse(instanceid, out var result)) return result;
                return 0;
            }
        }
        /// <summary>
        /// the asset ID given to the item after the trade completed
        /// </summary>
        [JsonIgnore] public uint u_new_assetid
        {
            get
            {
                if (uint.TryParse(new_assetid, out var result)) return result;
                return 0;
            }
        }
        /// <summary>
        /// the context ID the item was placed in
        /// </summary>
        [JsonIgnore] public ushort u_new_contextid
        {
            get
            {
                if (ushort.TryParse(new_contextid, out var result)) return result;
                return 0;
            }
        }
        /// <summary>
        /// if the trade has been rolled back, the new asset ID given in the rollback
        /// </summary>
        [JsonIgnore] public uint u_rollback_new_assetid
        {
            get
            {
                if (uint.TryParse(rollback_new_assetid, out var result)) return result;
                return 0;
            }
        }
        /// <summary>
        /// if the trade has been rolled back, the context ID the new asset was placed in
        /// </summary>
        [JsonIgnore] public ushort u_rollback_new_contextid
        {
            get
            {
                if (ushort.TryParse(rollback_new_contextid, out var result)) return result;
                return 0;
            }
        }
    }
}
