using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SilverBarricadeStructureTools.Models
{
    public class mHeightLimiter
    {
        [XmlAttribute("Enabled")]
        public bool Enabled{ get; set; }
        public int MaxHeight{ get; set; }
        public bool UseDynamicHeight{ get; set; }
        public int DynamicHeight{ get; set; }
        public bool EnabledHeightLimitedCommands{ get; set; }
        public int CommandsMinYToBlock{ get; set; }
        public List<string> BlockedCommands{ get; set; }
        public List<CustomHeightObject> CustomHeights{ get; set; }
    }

    public class CustomHeightObject
    {
        [XmlAttribute("ID")]
        public ushort ID{ get; set; }
        [XmlAttribute("MaxHeight")]
        public int MaxHeight{ get; set; }
        [XmlAttribute("DynamicHeight")]
        public int DynamicHeight{ get; set; }
    }
}
