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
    public static class RoadPlaceBlocking
    {
        public static void Execute(ushort id, Vector3 point, ref bool shouldAllow, ulong owner)
        {
            if (SBST.Instance.cfg.RoadPlaceBlocking.AllowedIds.Contains(id)) return;
            if (Physics.Raycast(new Ray(point, new Vector3(0, -1, 0)), out _, SBST.Instance.cfg.RoadPlaceBlocking.MinHeightAboveRoad, RayMasks.ENVIRONMENT))
            {
                shouldAllow = false;
                UnturnedChat.Say((CSteamID)owner, SBST.Instance.Translate("NoBuildHere"), SBST.Instance.MessageColor);
            }
        }
    }
}
