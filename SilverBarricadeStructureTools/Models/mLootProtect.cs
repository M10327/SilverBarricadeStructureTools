using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SilverBarricadeStructureTools.Models
{
    public class mLootProtect
    {
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }
        [XmlAttribute("AllowAllVulnerable")]
        public bool AllowAllVulnerable { get; set; }
        [XmlAttribute("Resize")]
        public int Resize { get; set; }
        [XmlAttribute("Height")]
        public int Height { get; set; }
        public List<ushort> AllowedIds { get; set; }
    }
}
