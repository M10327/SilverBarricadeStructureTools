using Rocket.API;
using Rocket.Unturned.Chat;
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
    public static class HeightLimiter
    {
        public static void Commands_OnExecuteCommand(IRocketPlayer player, IRocketCommand command, ref bool cancel)
        {
            if (player is ConsolePlayer) return;
            if (player == null) return;
            UnturnedPlayer p = player as UnturnedPlayer;
            if (p == null) return;
            if (p.Position.y > SBST.Instance.cfg.HeightLimiter.CommandsMinYToBlock)
            {
                if (SBST.Instance.cfg.HeightLimiter.BlockedCommands.Contains(command.Name.ToLower()))
                {
                    cancel = true;
                    UnturnedChat.Say(p, SBST.Instance.Translate("CommandBlocked", command.Name, SBST.Instance.cfg.HeightLimiter.CommandsMinYToBlock), SBST.Instance.MessageColor);
                }
            }
        }

        public static void CheckPlacement(CSteamID owner, Vector3 point, ref bool shouldAllow, ushort id)
        {
            var cfg = SBST.Instance.cfg.HeightLimiter;
            try
            {
                var p = UnturnedPlayer.FromCSteamID(owner);
                if (p == null) return;
                int max = cfg.MaxHeight;
                int dynMax = cfg.DynamicHeight;
                foreach (var cst in cfg.CustomHeights)
                {
                    if (cst.ID == id)
                    {
                        max = cst.MaxHeight;
                        dynMax = cst.DynamicHeight;
                        break;
                    }
                }
                if (point.y >= max)
                {
                    shouldAllow = false;
                    UnturnedChat.Say(p, SBST.Instance.Translate("MaxHeight", max), SBST.Instance.MessageColor);
                    return;
                }
                if (cfg.UseDynamicHeight)
                {
                    var mask = RayMasks.GROUND | RayMasks.GROUND2 | RayMasks.WATER | RayMasks.SMALL | RayMasks.MEDIUM | RayMasks.LARGE;
                    point.y += 5;
                    if (!Physics.Raycast(new Ray(point, Vector3.down), out _, dynMax, mask))
                    {
                        shouldAllow = false;
                        UnturnedChat.Say(owner, SBST.Instance.Translate("MaxHeightDynamic", dynMax), SBST.Instance.MessageColor);
                        return;
                    }
                }
            }
            catch (Exception ex) { Rocket.Core.Logging.Logger.LogError($"Caught Exception: {ex}"); }
        }
    }
}
