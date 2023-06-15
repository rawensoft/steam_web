using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWeb.API.Models.IPlayerService
{
    public record Customization
    {
        public ushort customization_type { get; set; }
        public bool active { get; set; } = false;
        public ushort level { get; set; }
        public List<CustomizationSlot> slots { get; set; } = new();
    }
}
