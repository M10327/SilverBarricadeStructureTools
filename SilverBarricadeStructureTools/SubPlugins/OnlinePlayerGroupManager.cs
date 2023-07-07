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
    public class OnlinePlayerGroupManager
    {
        public static void Events_OnPlayerConnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            player.Player.quests.groupIDChanged += Quests_groupIDChanged;
            if (!SBST.Instance.OnlinePlayers.Contains((ulong)player.CSteamID))
                SBST.Instance.OnlinePlayers.Add((ulong)player.CSteamID);
            AddGroup((ulong)player.Player.quests.groupID);
        }

        public static void Quests_groupIDChanged(PlayerQuests sender, CSteamID oldGroupID, CSteamID newGroupID)
        {
            RemoveGroup((ulong)oldGroupID, (ulong)sender.player.channel.owner.playerID.steamID);
            AddGroup((ulong)newGroupID);
        }

        public static void Events_OnPlayerDisconnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            player.Player.quests.groupIDChanged -= Quests_groupIDChanged;
            RemoveGroup((ulong)player.Player.quests.groupID, (ulong)player.CSteamID);
            if (SBST.Instance.OnlinePlayers.Contains((ulong)player.CSteamID))
                SBST.Instance.OnlinePlayers.Remove((ulong)player.CSteamID);
        }

        public static void AddGroup(ulong groupID)
        {
            if (groupID == 0) return;
            if (!SBST.Instance.OnlineGroups.Contains(groupID))
                SBST.Instance.OnlineGroups.Add(groupID);
        }

        public static void RemoveGroup(ulong groupID, ulong owner)
        {
            bool foundOthers = false;
            foreach (var steamPl in Provider.clients)
            {
                if ((ulong)steamPl.player.quests.groupID == groupID && (ulong)steamPl.playerID.steamID != owner)
                {
                    foundOthers = true;
                    break;
                }
            }
            if (!foundOthers)
            {
                SBST.Instance.OnlineGroups.Remove(groupID);
            }
        }
    }
}
