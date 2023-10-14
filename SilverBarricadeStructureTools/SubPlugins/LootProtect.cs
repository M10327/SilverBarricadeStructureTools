using Pathfinding;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SilverBarricadeStructureTools.SubPlugins
{
    public static class LootProtect
    {
        public static void CheckIfAllowed(ulong playerId, Vector3 position, ref bool shouldAllow, ushort itemId, bool breakableWithLowcal, string Name)
        {
            if (!LevelNavigation.tryGetNavigation(position, out byte nav)) return;
            var bounds = ((RecastGraph)AstarPath.active.graphs[(int)nav]).forcedBounds;
            if (position.y > (bounds.center.y + SBST.Instance.cfg.LootProtect.Height)) return;
            bounds.Expand(SBST.Instance.cfg.LootProtect.Resize);
            if (!bounds.ContainsXZ(position)) return;
            if (breakableWithLowcal && SBST.Instance.cfg.LootProtect.AllowAllVulnerable) return;
            if (SBST.Instance.cfg.LootProtect.AllowedIds.Contains(itemId)) return;
            UnturnedChat.Say((CSteamID)playerId, SBST.Instance.Translate("LootProtect", Name), SBST.Instance.MessageColor);
            shouldAllow = false;
        }
    }
}
