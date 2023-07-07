using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBarricadeStructureTools.SubPlugins
{
    public static class OfflineRaidProt
    {
        public static void ModifyDamage(ref ushort damage, ref bool shouldAllow, ulong owner, ulong group)
        {
            float multi = 1;
            if (!SBST.Instance.OnlinePlayers.Contains(owner) && !SBST.Instance.OnlineGroups.Contains(group))
            {
                // no one is online
                multi = SBST.Instance.cfg.OfflineRaidProt.OfflineMulti;
            }
            else
            {
                // someone is online
                multi = SBST.Instance.cfg.OfflineRaidProt.OnlineMulti;
            }

            damage = (ushort)(damage * multi);
            if (damage < 1) shouldAllow = false;
        }
    }
}
