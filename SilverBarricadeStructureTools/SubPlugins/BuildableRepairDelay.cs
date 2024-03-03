using Rocket.Unturned.Chat;
using Rocket.Unturned.Items;
using Rocket.Unturned.Player;
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
            var cfg = SBST.Instance.cfg;
            if (!checkIfCanRepair(id, out long delta))
            {
                shouldAllow = false;
                if ((ulong)instigatorSteamID < 1000 || instigatorSteamID == Provider.server || instigatorSteamID == null) return;
                long timeLeft = cfg.BuildableRepairDelay.RepairDelaySeconds - delta;
                if (cfg.BuildableRepairDelay.UseChat)
                    UnturnedChat.Say(instigatorSteamID, SBST.Instance.Translate("RepairDelay", timeLeft.ToString()), SBST.Instance.MessageColor);
                if (cfg.BuildableRepairDelay.UseUI)
                {
                    UnturnedPlayer p = UnturnedPlayer.FromCSteamID(instigatorSteamID);
                    EffectManager.askEffectClearByID(cfg.BuildableRepairDelay.UiId, p.Player.channel.owner.transportConnection);
                    EffectManager.sendUIEffect(cfg.BuildableRepairDelay.UiId, (short)(cfg.BuildableRepairDelay.UiId + 10), p.Player.channel.owner.transportConnection, true, SBST.Instance.Translate("RepairDelay", timeLeft.ToString()));
                }
            }
        }

        public static bool checkIfCanRepair(NetId id, out long delta)
        {
            delta = 0;
            if (!SBST.Instance.TimeLastDamaged.ContainsKey(id)) return true;
            delta = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - SBST.Instance.TimeLastDamaged[id];
            if (delta < SBST.Instance.cfg.BuildableRepairDelay.RepairDelaySeconds) return false;
            else return true;
        }

        public static void SetLastDamaged(NetId id)
        {
            SBST.Instance.TimeLastDamaged[id] = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
