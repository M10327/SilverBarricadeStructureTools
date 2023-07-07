using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SilverBarricadeStructureTools.Models
{
    public class mDecay
    {
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }
        public bool DamageWhileOwnerOnline { get; set; }
        public float DamagePercent { get; set; }
        public float HealPercent { get; set; }
        public int IntervalSeconds { get; set; }
        public List<ulong> DecayIgnoreOwnerGroupIds { get; set; }
    }
}
