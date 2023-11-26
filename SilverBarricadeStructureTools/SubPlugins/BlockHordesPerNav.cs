using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;

namespace SilverBarricadeStructureTools.SubPlugins
{
    public static class BlockHordesPerNav
    {
        public static void Check(ItemBarricadeAsset asset, Vector3 point, ref bool shouldAllow, ulong owner)
        {
            if (asset is not ItemBeaconAsset) return;
            LevelNavigation.tryGetBounds(point, out byte nav);
            if (SBST.Instance.cfg.BlockHordesNavList.Contains(nav))
            {
                shouldAllow = false;
                UnturnedChat.Say((CSteamID)owner, SBST.Instance.Translate("BlockHorde"), SBST.Instance.MessageColor);
            }
        }
    }
}
