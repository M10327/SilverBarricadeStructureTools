using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SilverBarricadeStructureTools.Models
{
    public class mVehicleNoPlaceOn
    {
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }
        public List<ushort> VehicleIDs { get; set; }
    }
}
