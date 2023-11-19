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
        public List<string> IgnoreBuildTypes { get; set; }
        public List<ushort> Ids { get; set; }
    }
}
