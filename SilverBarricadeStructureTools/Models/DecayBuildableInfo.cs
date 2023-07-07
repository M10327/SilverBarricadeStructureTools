using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SilverBarricadeStructureTools.Models
{
    public class DecayBuildableInfo
    {
        public DecayBuildableInfo(bool isFarm, Vector3 point, ulong owner, ulong group, ushort healthCurrent, ushort healthMax)
        {
            IsFarm = isFarm;
            Point = point;
            Owner = owner;
            Group = group;
            HealthCurrent = healthCurrent;
            HealthMax = healthMax;
        }

        public bool IsFarm { get; set; }
        public Vector3 Point { get; set; }
        public ulong Owner { get; set; }
        public ulong Group { get; set; }
        public ushort HealthCurrent { get; set; }
        public ushort HealthMax { get; set; }
    }
}
