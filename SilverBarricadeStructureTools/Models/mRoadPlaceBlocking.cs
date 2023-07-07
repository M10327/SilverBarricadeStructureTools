using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBarricadeStructureTools.Models
{
    public class mRoadPlaceBlocking
    {
        public bool Enabled { get; set; }
        public int MinHeightAboveRoad { get; set; }
        public List<ulong> AllowedIds { get; set; }
    }
}
