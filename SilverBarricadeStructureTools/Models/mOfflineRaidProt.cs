using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SilverBarricadeStructureTools.Models
{
    public class mOfflineRaidProt
    {
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }
        public float OnlineMulti { get; set; }
        public float OfflineMulti { get; set; }
    }
}
