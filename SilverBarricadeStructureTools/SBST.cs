using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using SilverBarricadeStructureTools.SubPlugins;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SilverBarricadeStructureTools
{
    public class SBST : RocketPlugin<Config>
    {
        public Config cfg { get; set; }
        public static SBST Instance { get; private set; }
        public UnityEngine.Color MessageColor { get; set; }
        private static System.Timers.Timer AutoCheckTimer { get; set; }
        private static System.Timers.Timer DecayTimer { get; set; }
        public List<ulong> OnlinePlayers { get; set; }
        public List<ulong> OnlineGroups { get; set; }
        protected override void Load()
        {
            Instance = this;
            cfg = Configuration.Instance;
            MessageColor = (Color)UnturnedChat.GetColorFromHex(cfg.MessageColor);
            BarricadeManager.onDeployBarricadeRequested += BarricadePlace;
            BarricadeManager.onDamageBarricadeRequested += BarricadeDamage;
            StructureManager.onDeployStructureRequested += StructurePlace;
            StructureManager.onDamageStructureRequested += StructureDamage;
            Patches.PatchAll();
            Patches.OnBarricadeDestroying += Patches_OnBarricadeDestroying;
            Patches.OnStructureDestroying += Patches_OnStructureDestroying;
            if (cfg.HeightLimiter.EnabledHeightLimitedCommands) R.Commands.OnExecuteCommand += HeightLimiter.Commands_OnExecuteCommand;
            U.Events.OnPlayerConnected += OnlinePlayerGroupManager.Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnlinePlayerGroupManager.Events_OnPlayerDisconnected;

            // Auto Toggle and Gen Timer
            AutoCheckTimer = new System.Timers.Timer(cfg.AutoToggleAndUnlimited.SecondsBetweenChecks * 1000);
            AutoCheckTimer.Elapsed += AutoToggleAndUnlimited.AutoCheckTimer_Elapsed;
            AutoCheckTimer.AutoReset = true;
            AutoCheckTimer.Enabled = true;

            // Decay Timer
            DecayTimer = new System.Timers.Timer(cfg.Decay.IntervalSeconds * 1000);
            DecayTimer.Elapsed += Decay.DecayTimer_Elapsed;
            DecayTimer.AutoReset = true;
            DecayTimer.Enabled = true;

            OnlineGroups = new List<ulong>();
            OnlinePlayers = new List<ulong>();
            foreach (var pl in Provider.clients)
            {
                OnlinePlayerGroupManager.Events_OnPlayerConnected(UnturnedPlayer.FromSteamPlayer(pl));
            }

            Rocket.Core.Logging.Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded");
        }

        // todo:
        // raid logs
        // option for offline raid prot to require claim flags

        private void Patches_OnStructureDestroying(StructureDrop drop)
        {

        }

        private void Patches_OnBarricadeDestroying(BarricadeDrop drop)
        {

        }

        private void StructureDamage(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            var structure = StructureManager.FindStructureByRootTransform(structureTransform);
            var data = structure.GetServersideData();
            Unbreakables.Execute(data.owner, data.group, ref shouldAllow, structure.asset.itemName, null, instigatorSteamID);
            if (cfg.OfflineRaidProt.Enabled) OfflineRaidProt.ModifyDamage(ref pendingTotalDamage, ref shouldAllow, data.owner, data.group);
        }

        private void StructurePlace(Structure structure, ItemStructureAsset asset, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            if (CheckIfBypassPlacements(owner)) return;
            if (cfg.RoadPlaceBlocking.Enabled)
                RoadPlaceBlocking.Execute(asset.id, point, ref shouldAllow, owner);
            if (cfg.HeightLimiter.Enabled)
                HeightLimiter.CheckPlacement((CSteamID)owner, point, ref shouldAllow, asset.id);
        }

        private void BarricadeDamage(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            var barricade = BarricadeManager.FindBarricadeByRootTransform(barricadeTransform);
            var data = barricade.GetServersideData();
            Unbreakables.Execute(data.owner, data.group, ref shouldAllow, barricade.asset.itemName, barricade.asset.build, instigatorSteamID);
            if (cfg.OfflineRaidProt.Enabled) OfflineRaidProt.ModifyDamage(ref pendingTotalDamage, ref shouldAllow, data.owner, data.group);
            if (barricadeTransform.parent != null && barricadeTransform.parent.TryGetComponent<InteractableVehicle>(out InteractableVehicle vehicle))
            {
                
            }
            else
            {
                
            }
        }

        private void BarricadePlace(Barricade barricade, ItemBarricadeAsset asset, Transform hit, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            if (CheckIfBypassPlacements(owner)) return;
            if (hit != null && hit.TryGetComponent<InteractableVehicle>(out InteractableVehicle vehicle))
            {
                if (cfg.VehicleBarricadeBlacklist.Enabled)
                    VehicleBarricadeBlacklist.Execute(asset, owner, ref shouldAllow);
                if (cfg.BlockBuildingOnOthersVehicles)
                    VehicleBuildProtection.Execute(asset, owner, group, ref shouldAllow, vehicle);
            }
            else
            {
                if (cfg.RoadPlaceBlocking.Enabled)
                    RoadPlaceBlocking.Execute(asset.id, point, ref shouldAllow, owner);
                if (cfg.HeightLimiter.Enabled)
                    HeightLimiter.CheckPlacement((CSteamID)owner, point, ref shouldAllow, asset.id);
            }
        }

        public bool CheckIfBypassPlacements(ulong owner)
        {
            if (!cfg.BypassPlacementsAllow) return false;
            var p = UnturnedPlayer.FromCSteamID((CSteamID)owner);
            if (p.HasPermission(cfg.BypassPlacementsPerm) || p.IsAdmin) return true;
            else return false;
        }

        protected override void Unload()
        {
            BarricadeManager.onDeployBarricadeRequested -= BarricadePlace;
            BarricadeManager.onDamageBarricadeRequested -= BarricadeDamage;
            StructureManager.onDeployStructureRequested -= StructurePlace;
            StructureManager.onDamageStructureRequested -= StructureDamage;
            Patches.UnpatchAll();
            Patches.OnBarricadeDestroying -= Patches_OnBarricadeDestroying;
            Patches.OnStructureDestroying -= Patches_OnStructureDestroying;
            if (cfg.HeightLimiter.EnabledHeightLimitedCommands) R.Commands.OnExecuteCommand -= HeightLimiter.Commands_OnExecuteCommand;
            U.Events.OnPlayerConnected -= OnlinePlayerGroupManager.Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnlinePlayerGroupManager.Events_OnPlayerDisconnected;

            AutoCheckTimer.Stop();
            AutoCheckTimer.Elapsed -= AutoToggleAndUnlimited.AutoCheckTimer_Elapsed;
            DecayTimer.Stop();
            DecayTimer.Elapsed -= Decay.DecayTimer_Elapsed;

            Rocket.Core.Logging.Logger.Log($"{Name} {Assembly.GetName().Version} has been unloaded");
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "PlacementBlocked", "You cannot place {0} on vehicles!" },
            { "NotYourVehicle", "That is not your vehicle!" },
            { "NoBuildHere", "You may not build on roads!" },
            { "MayNotDamage", "This {0} is protected from all damage!" },
            { "MaxHeight", "You cannot build over the max height limit! ({0}m)" },
            { "MaxHeightDynamic", "You cannot build more than {0}m above the ground!" },
            { "CommandBlocked", "You cannot use the {0} command above {1}m!" }
        };
    }
}
