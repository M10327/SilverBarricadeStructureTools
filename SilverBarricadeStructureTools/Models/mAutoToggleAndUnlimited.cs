using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SilverBarricadeStructureTools.Models
{
    public class mAutoToggleAndUnlimited
    {
        [XmlAttribute("Enabled")]
        public bool Enabled;
        public int SecondsBetweenChecks;
        public List<ulong> Generators;
        public List<ulong> Lights;
        public List<ulong> Oxygenators;
        public List<ulong> Fires;
    }
}