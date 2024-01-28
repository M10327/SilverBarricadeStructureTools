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
using SilverBarricadeStructureTools.Models;
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
        private static System.Timers.Timer RaidLogTimer { get; set; }
        public List<ulong> OnlinePlayers { get; set; }
        public List<ulong> OnlineGroups { get; set; }
        public Dictionary<NetId, long> TimeLastDamaged { get; set; }
        public List<RaidInstance> RaidInstances;
        protected override void Load()
        {
            Instance = this;
            cfg = Configuration.Instance;
            MessageColor = (Color)UnturnedChat.GetColorFromHex(cfg.MessageColor);
            BarricadeManager.onDeployBarricadeRequested += BarricadePlace;
            BarricadeManager.onDamageBarricadeRequested += BarricadeDamage;
            StructureManager.onDeployStructureRequested += StructurePlace;
            StructureManager.onDamageStructureRequested += StructureDamage;
            BarricadeManager.OnRepairRequested += BarricadeRepair;
            StructureManager.OnRepairRequested += StructureRepair;
            InteractableFarm.OnHarvestRequested_Global += InteractableFarm_OnHarvestRequested_Global;
            Patches.PatchAll();
            Patches.OnBarricadeDestroying += Patches_OnBarricadeDestroying;
            Patches.OnStructureDestroying += Patches_OnStructureDestroying;
            if (cfg.HeightLimiter.EnabledHeightLimitedCommands) R.Commands.OnExecuteCommand += HeightLimiter.Commands_OnExecuteCommand;
            U.Events.OnPlayerConnected += OnlinePlayerGroupManager.Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnlinePlayerGroupManager.Events_OnPlayerDisconnected;
            TimeLastDamaged = new Dictionary<NetId, long>();
            RaidInstances = new List<RaidInstance>();

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

            // Raid Log Timer
            RaidLogTimer = new System.Timers.Timer(cfg.raidLogs.MaxSecondsBetweenPosts * 1000);
            RaidLogTimer.Elapsed += RaidLogs.TimeElapsed;
            RaidLogTimer.AutoReset = true;
            RaidLogTimer.Enabled = true;

            OnlineGroups = new List<ulong>();
            OnlinePlayers = new List<ulong>();
            foreach (var pl in Provider.clients)
            {
                OnlinePlayerGroupManager.Events_OnPlayerConnected(UnturnedPlayer.FromSteamPlayer(pl));
            }
            List<string> types = Enum.GetValues(typeof(EBuild)).Cast<EBuild>().ToList().ConvertAll(x => x.ToString());
            Rocket.Core.Logging.Logger.Log($"Valid Build Types: {string.Join(",", types)}");
            Rocket.Core.Logging.Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded");
        }

        private void InteractableFarm_OnHarvestRequested_Global(InteractableFarm harvestable, SteamPlayer instigatorPlayer, ref bool shouldAllow)
        {
            if (cfg.AutoReplantEnabled) AutoReplant.Execute(harvestable, instigatorPlayer, ref shouldAllow);
        }

        private void StructureRepair(CSteamID instigatorSteamID, Transform structureTransform, ref float pendingTotalHealing, ref bool shouldAllow)
        {
            var structure = StructureManager.FindStructureByRootTransform(structureTransform);
            var data = structure.GetServersideData();
            if (cfg.BuildableRepairDelay.Enabled) 
                BuildableRepairDelay.CheckIfCanRepair(structure.GetNetId(), instigatorSteamID, ref shouldAllow);
        }

        private void BarricadeRepair(CSteamID instigatorSteamID, Transform barricadeTransform, ref float pendingTotalHealing, ref bool shouldAllow)
        {
            var barricade = BarricadeManager.FindBarricadeByRootTransform(barricadeTransform);
            var data = barricade.GetServersideData();
            if (cfg.BuildableRepairDelay.Enabled)
                BuildableRepairDelay.CheckIfCanRepair(barricade.GetNetId(), instigatorSteamID, ref shouldAllow);
        }

        // todo:
        // option for offline raid prot to require claim flags
        // fix auto replant still

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
            if (cfg.BuildableRepairDelay.Enabled)
                BuildableRepairDelay.SetLastDamaged(structure.GetNetId());
            if (cfg.ProtectionClaims.Enabled)
                ProtectionClaims.CheckProtected((ulong)instigatorSteamID, structureTransform.position, ref shouldAllow, structure.asset, null);
            if (cfg.raidLogs.WebhookUrl.Length > 25)
                RaidLogs.CheckDamagedStructure(instigatorSteamID, structureTransform, ref pendingTotalDamage, ref shouldAllow);

        }

        private void StructurePlace(Structure structure, ItemStructureAsset asset, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            if (CheckIfBypassPlacements(owner)) return;
            if (cfg.RoadPlaceBlocking.Enabled)
                RoadPlaceBlocking.Execute(asset.id, point, ref shouldAllow, owner);
            if (cfg.HeightLimiter.Enabled)
                HeightLimiter.CheckPlacement((CSteamID)owner, point, ref shouldAllow, asset.id);
            if (cfg.LootProtect.Enabled)
                LootProtect.CheckIfAllowed(owner, point, ref shouldAllow, asset.id, asset.isVulnerable, asset.itemName);
            if (cfg.LocalBuildLimiter.Enabled)
                LocalBuildLimiter.CheckStructures(owner, point, ref shouldAllow, asset.id);
            if (cfg.PrintPlacementInfoToConsole)
                PrintPlacementInfo.Structure(asset, point, angle_x, angle_y, angle_z, owner, group);
        }

        private void BarricadeDamage(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            var barricade = BarricadeManager.FindBarricadeByRootTransform(barricadeTransform);
            var data = barricade.GetServersideData();
            Unbreakables.Execute(data.owner, data.group, ref shouldAllow, barricade.asset.itemName, barricade.asset.build, instigatorSteamID);
            if (cfg.OfflineRaidProt.Enabled && data.barricade.asset.build != EBuild.FARM) OfflineRaidProt.ModifyDamage(ref pendingTotalDamage, ref shouldAllow, data.owner, data.group);
            if (barricadeTransform.parent != null && barricadeTransform.parent.TryGetComponent<InteractableVehicle>(out InteractableVehicle vehicle))
            {
                // only on vehicles
            }
            else
            {
                // only off vehicles
                if (cfg.BuildableRepairDelay.Enabled)
                    BuildableRepairDelay.SetLastDamaged(barricade.GetNetId());
                if (cfg.ProtectionClaims.Enabled)
                    ProtectionClaims.CheckProtected((ulong)instigatorSteamID, barricadeTransform.position, ref shouldAllow, null, barricade.asset);
                if (cfg.raidLogs.WebhookUrl.Length > 25)
                    RaidLogs.CheckDamagedBarricade(instigatorSteamID, barricadeTransform, ref pendingTotalDamage, ref shouldAllow);
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
                if (cfg.VehicleNoPlaceOn.Enabled)
                    VehicleNoPlaceOn.Execute(vehicle, owner, ref shouldAllow);
                if (cfg.VehicleBuildCap.Enabled)
                    VehicleBuildCap.CheckCap(vehicle, owner, ref shouldAllow, barricade.asset.id);
                if (cfg.PrintPlacementInfoToConsole)
                    PrintPlacementInfo.Barricade(asset, point, angle_x, angle_y, angle_z, owner, group, true, vehicle);
            }
            else
            {
                if (cfg.RoadPlaceBlocking.Enabled)
                    RoadPlaceBlocking.Execute(asset.id, point, ref shouldAllow, owner);
                if (cfg.HeightLimiter.Enabled)
                    HeightLimiter.CheckPlacement((CSteamID)owner, point, ref shouldAllow, asset.id);
                if (cfg.LootProtect.Enabled) 
                    LootProtect.CheckIfAllowed(owner, point, ref shouldAllow, asset.id, asset.isVulnerable, asset.itemName);
                if (cfg.LocalBuildLimiter.Enabled)
                    LocalBuildLimiter.CheckBarricades(owner, point, ref shouldAllow, asset.id);
                if (cfg.PrintPlacementInfoToConsole)
                    PrintPlacementInfo.Barricade(asset, point, angle_x, angle_y, angle_z, owner, group, false, null);
                if (cfg.BlockHordesNavList.Count > 0)
                    BlockHordesPerNav.Check(asset, point, ref shouldAllow, owner);
            }
        }

        public bool CheckIfBypassPlacements(ulong owner)
        {
            if (owner == (ulong)Provider.server) return true;
            if (owner == 0) return true;
            if (!cfg.BypassPlacementsAllow) return false;
            var p = UnturnedPlayer.FromCSteamID((CSteamID)owner);
            if (p == null) return true;
            if (p.HasPermission(cfg.BypassPlacementsPerm) || p.IsAdmin) return true;
            else return false;
        }

        protected override void Unload()
        {
            BarricadeManager.onDeployBarricadeRequested -= BarricadePlace;
            BarricadeManager.onDamageBarricadeRequested -= BarricadeDamage;
            StructureManager.onDeployStructureRequested -= StructurePlace;
            StructureManager.onDamageStructureRequested -= StructureDamage;
            BarricadeManager.OnRepairRequested -= BarricadeRepair;
            StructureManager.OnRepairRequested -= StructureRepair;
            InteractableFarm.OnHarvestRequested_Global -= InteractableFarm_OnHarvestRequested_Global;
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
            RaidLogTimer.Stop();
            RaidLogTimer.Elapsed -= RaidLogs.TimeElapsed;
            RaidLogs.PublishRaidLogsShutdown();
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
            { "CommandBlocked", "You cannot use the {0} command above {1}m!" },
            { "PlacementBlockedVehicle", "You cannot build on {0}" },
            { "RepairDelay", "You must wait {0} more seconds to repair this!" },
            { "VehicleBuildCap", "You cannot place more than {0} {1} on a vehicle." },
            { "LootProtect", "You cannot place {0} near loot spawns." },
            { "LocalBuildLimit", "You cannot place more than {0} {1} within {2}m of each other" },
            { "BlockHorde", "You cannot place horde beacons here" }
        };
    }
}
