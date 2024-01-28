using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBarricadeStructureTools.Models
{
    public class mRaidLogs
    {
        public string WebhookUrl { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public int RaidsPerEmbed { get; set; }
        public int MaxSecondsBetweenPosts { get; set; }
        public List<ushort> IgnoreWeaponIds { get; set; }
    }
}
