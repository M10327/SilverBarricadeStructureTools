using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SilverBarricadeStructureTools.Models
{
    public class mProtectionClaims
    {
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }
        [XmlAttribute("IgnoreStorage")]
        public bool IgnoreStorage { get; set; }
        [XmlAttribute("IgnoreSentries")]
        public bool IgnoreSentries { get; set; }
        [XmlAttribute("IgnoreDoors")]
        public bool IgnoreDoors { get; set; }
        public List<ushort> Ids { get; set; }
    }
}
