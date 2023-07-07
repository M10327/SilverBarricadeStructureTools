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
    public static class VehicleBuildProtection
    {
        public static void Execute(ItemBarricadeAsset asset, ulong owner, ulong group, ref bool shouldAllow, InteractableVehicle vehicle)
        {
            if (vehicle.isLocked && !((ulong)vehicle.lockedGroup == group || (ulong)vehicle.lockedOwner == owner))
            {
                shouldAllow = false;
                UnturnedChat.Say((CSteamID)owner, SBST.Instance.Translate("NotYourVehicle", asset.itemName), SBST.Instance.MessageColor);
                return;
            }
        }
    }
}
