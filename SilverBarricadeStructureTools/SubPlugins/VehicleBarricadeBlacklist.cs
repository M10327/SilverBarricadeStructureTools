using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SilverBarricadeStructureTools.SubPlugins
{
    public static class VehicleBarricadeBlacklist
    {
        public static void Execute(ItemBarricadeAsset asset, ulong owner, ref bool shouldAllow)
        {
            if (SBST.Instance.cfg.VehicleBarricadeBlacklist.Barricades.Contains(asset.id))
            {
                shouldAllow = false;
                UnturnedChat.Say((CSteamID)owner, SBST.Instance.Translate("PlacementBlocked", asset.itemName), SBST.Instance.MessageColor);
                return;
            }
        }
    }
}
