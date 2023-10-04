using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SilverBarricadeStructureTools.Models
{
    public class mVehicleBuildCap
    {
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }
        public List<VehicleBuildCapObject> BuildCaps { get; set; }
    }

    public class VehicleBuildCapObject
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        public List<ushort> Ids { get; set; }
        [XmlAttribute("MaxAllowed")]
        public int MaxAllowed { get; set; }
    }
}
