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
    public static class VehicleNoPlaceOn
    {
        public static void Execute(InteractableVehicle vehicle, ulong owner, ref bool shouldAllow)
        {
            if (SBST.Instance.cfg.VehicleNoPlaceOn.VehicleIDs.Contains(vehicle.id))
            {
                shouldAllow = false;
                UnturnedChat.Say((CSteamID)owner, SBST.Instance.Translate("PlacementBlockedVehicle", vehicle.asset.FriendlyName), SBST.Instance.MessageColor);
                return;
            }
        }
    }
}
