using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SilverBarricadeStructureTools.Models
{
    public class BarricadeStore
    {
        public Vector3 Pos { get; set; }
        public ulong Owner { get; set; }
        public ulong Group { get; set; }
        public Transform Transform { get; set; }
    }
}
