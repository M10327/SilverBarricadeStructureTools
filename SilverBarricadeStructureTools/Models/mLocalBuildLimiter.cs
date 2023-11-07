using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SilverBarricadeStructureTools.Models
{
    public class mLocalBuildLimiter
    {
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }
        public List<BuildLimitObject> Barricades { get; set; }
        public List<BuildLimitObject> Structures { get; set; }
    }

    public class BuildLimitObject
    {

        [XmlAttribute("CheckRange")]
        public int CheckRange { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("Limit")]
        public int Limit { get; set; }
        public List<ushort> Ids { get; set; }
    }
}
