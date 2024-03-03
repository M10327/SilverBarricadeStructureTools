using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBarricadeStructureTools.SubPlugins
{
    public static class Unbreakables
    {
        public static void Execute(ulong owner, ulong group, ref bool shouldAllow, string name, EBuild? buildType, CSteamID instigatorSteamID)
        {
            if (buildType != null && buildType == EBuild.FARM) return;
            if (SBST.Instance.cfg.UnbreakablesIds.Contains(owner) || SBST.Instance.cfg.UnbreakablesIds.Contains(group))
            {
                shouldAllow = false;
                if ((ulong)instigatorSteamID < 1000 || instigatorSteamID == Provider.server || instigatorSteamID == null) return;
                UnturnedChat.Say(instigatorSteamID, SBST.Instance.Translate("MayNotDamage", name), SBST.Instance.MessageColor);
            }
        }
    }
}
