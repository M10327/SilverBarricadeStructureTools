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
            if (!SBST.Instance.OnlinePlayers.Contains(owner) && !SBST.Instance.OnlineGroups.Contains(group))
            {
                // no one is online
                damage = (ushort)(damage * SBST.Instance.cfg.OfflineRaidProt.OfflineMulti);
            }
            else
            {
                // someone is online
                damage = (ushort)(damage * SBST.Instance.cfg.OfflineRaidProt.OnlineMulti);
            }

            if (damage < 1) shouldAllow = false;
        }
    }
}
