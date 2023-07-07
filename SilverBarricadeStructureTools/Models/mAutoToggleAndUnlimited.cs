using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBarricadeStructureTools.Models
{
    public class mAutoToggleAndUnlimited
    {
        public bool Enabled;
        public int SecondsBetweenChecks;
        public List<ulong> Generators;
        public List<ulong> Lights;
        public List<ulong> Oxygenators;
        public List<ulong> Fires;
    }
}