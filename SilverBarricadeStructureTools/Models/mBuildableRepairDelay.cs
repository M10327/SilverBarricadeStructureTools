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
        public int RepairDelaySeconds { get; set; }
        public bool UseChat { get; set; }
        public bool UseUI { get; set; }
        public ushort UiId { get; set; }
    }
}
