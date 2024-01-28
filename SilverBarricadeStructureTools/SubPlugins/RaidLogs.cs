using Rocket.Core.Utils;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmyMySherbet.DiscordWebhooks;
using ShimmyMySherbet.DiscordWebhooks.Models;
using SilverBarricadeStructureTools.Models;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;

namespace SilverBarricadeStructureTools.SubPlugins
{
    public static class RaidLogs
    {
        
        public static void CheckDamagedStructure(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow)
        {
            var b = StructureManager.FindStructureByRootTransform(structureTransform).GetServersideData();
            if (b.structure.health - pendingTotalDamage <= 0 && shouldAllow)
            {
                if (instigatorSteamID != CSteamID.Nil && instigatorSteamID != Provider.server && instigatorSteamID != (CSteamID)0)
                    AddRaidInstance(instigatorSteamID, structureTransform.position, b.structure.asset.itemName, b.owner);
            }
        }

        public static void CheckDamagedBarricade(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow)
        {
            var t = BarricadeManager.FindBarricadeByRootTransform(barricadeTransform).GetServersideData();
            if (t.barricade.health - pendingTotalDamage <= 0 && shouldAllow)
            {
                if (instigatorSteamID != CSteamID.Nil && instigatorSteamID != Provider.server && instigatorSteamID != (CSteamID)0)
                    AddRaidInstance(instigatorSteamID, barricadeTransform.position, t.barricade.asset.itemName, t.owner);
            }
        }

        public static void AddRaidInstance(CSteamID instigatorSteamID, Vector3 position, string itemName, ulong owner)
        {
            var p = UnturnedPlayer.FromCSteamID(instigatorSteamID);
            if (p == null) return;
            string weaponName = "unarmed";
            if (p.Player.equipment != null && p.Player.equipment.asset != null)
            {
                if (SBST.Instance.cfg.raidLogs.IgnoreWeaponIds.Contains(p.Player.equipment.asset.id)) return;
                weaponName = p.Player.equipment.asset.itemName;
            }
            var raider = new Raider((ulong)instigatorSteamID, p.CharacterName, weaponName);
            var raid = new RaidInstance(owner, raider, position, itemName, DateTime.Now);
            SBST.Instance.RaidInstances.Add(raid);
            if (SBST.Instance.RaidInstances.Count >= SBST.Instance.cfg.raidLogs.RaidsPerEmbed)
            {
                PublishRaidLogs(SBST.Instance.RaidInstances.ToList());
                SBST.Instance.RaidInstances.Clear();
            }
        }

        public static void TimeElapsed(object sender, ElapsedEventArgs e)
        {
            if (SBST.Instance.cfg.raidLogs.WebhookUrl.Length > 25)
            {
                TaskDispatcher.QueueOnMainThread(() => ForcePublish());
            }
        }

        public static void ForcePublish()
        {
            PublishRaidLogs(SBST.Instance.RaidInstances.ToList());
            SBST.Instance.RaidInstances.Clear();
        }


        public static async void PublishRaidLogs(List<RaidInstance> list)
        {
            if (list.Count < 1) return;
            var cfg = SBST.Instance.cfg.raidLogs;
            WebhookMessage message = new WebhookMessage()
                .WithAvatar(cfg.IconUrl)
                .WithUsername(cfg.Name)
                .PassEmbed()
                .WithDescription($"{string.Join("\n", list.ConvertAll(x => x.GetString()))}")
                .Finalize();
            await DiscordWebhookService.PostMessageAsync(cfg.WebhookUrl, message);
        }

        public static void PublishRaidLogsShutdown()
        {
            var list = SBST.Instance.RaidInstances;
            if (list.Count < 1) return;
            var cfg = SBST.Instance.cfg.raidLogs;
            WebhookMessage message = new WebhookMessage()
                .WithAvatar(cfg.IconUrl)
                .WithUsername(cfg.Name)
                .PassEmbed()
                .WithDescription($"{string.Join("\n", list.ConvertAll(x => x.GetString()))}")
                .Finalize();
            DiscordWebhookService.PostMessageAsync(cfg.WebhookUrl, message);
        }
    }
}
