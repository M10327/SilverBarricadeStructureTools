using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SilverBarricadeStructureTools.Models
{
    public class mBuildableRepairDelay
    {
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }
        [XmlAttribute("RepairDelay")]
        public int RepairDelaySeconds { get; set; }
    }
}
