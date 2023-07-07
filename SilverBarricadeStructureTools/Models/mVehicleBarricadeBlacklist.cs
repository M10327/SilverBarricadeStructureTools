using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBarricadeStructureTools.Models
{
    public class mVehicleBarricadeBlacklist
    {
        public bool Enabled { get; set; }
        public List<ushort> Barricades { get; set; }
    }
}
