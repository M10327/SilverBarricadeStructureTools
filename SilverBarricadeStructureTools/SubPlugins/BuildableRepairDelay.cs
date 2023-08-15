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
    public static class BuildableRepairDelay
    {
        public static void CheckIfCanRepair(NetId id, CSteamID instigatorSteamID, ref bool shouldAllow)
        {
            if (!SBST.Instance.TimeLastDamaged.ContainsKey(id)) return;
            long delta = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - SBST.Instance.TimeLastDamaged[id];
            if (delta < SBST.Instance.cfg.BuildableRepairDelay.RepairDelaySeconds)
            {
                shouldAllow = false;
                long timeLeft = SBST.Instance.cfg.BuildableRepairDelay.RepairDelaySeconds - delta;
                UnturnedChat.Say(instigatorSteamID, SBST.Instance.Translate("RepairDelay", timeLeft.ToString()), SBST.Instance.MessageColor);
            }
        }

        public static void SetLastDamaged(NetId id)
        {
            SBST.Instance.TimeLastDamaged[id] = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
