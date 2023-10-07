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
    public static class VehicleBuildCap
    {
        public static void CheckCap(InteractableVehicle vehicle, ulong owner, ref bool shouldAllow, ushort id)
        {
            BarricadeRegion region;
            BarricadeManager.tryGetPlant(vehicle.transform, out byte _, out byte _, out ushort _, out region);
            Dictionary<ulong, int> barricadeCount = new Dictionary<ulong, int>();
            foreach (var b in region.drops)
            {
                if (!barricadeCount.ContainsKey(b.asset.id))
                    barricadeCount[b.asset.id] = 1;
                else
                    barricadeCount[b.asset.id]++;
            }
            foreach (var check in SBST.Instance.cfg.VehicleBuildCap.BuildCaps)
            {
                if (!check.Ids.Contains(id)) continue;
                int amount = 0;
                foreach (var checkId in check.Ids)
                {
                    if (barricadeCount.ContainsKey(checkId))
                        amount += barricadeCount[checkId];
                }
                if (amount == check.MaxAllowed)
                {
                    shouldAllow = false;
                    UnturnedChat.Say((CSteamID)owner, SBST.Instance.Translate("VehicleBuildCap", check.MaxAllowed.ToString(), check.Name), SBST.Instance.MessageColor);
                    return;
                }
            }
        }
    }
}
