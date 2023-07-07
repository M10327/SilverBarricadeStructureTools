using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SilverBarricadeStructureTools.Models
{
    public class mRoadPlaceBlocking
    {
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }
        public int MinHeightAboveRoad { get; set; }
        public List<ulong> AllowedIds { get; set; }
    }
}
