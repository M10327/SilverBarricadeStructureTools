using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
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

            // auto check timer
            AutoCheckTimer = new System.Timers.Timer(cfg.AutoToggleAndUnlimited.SecondsBetweenChecks * 1000);
            AutoCheckTimer.Elapsed += AutoToggleAndUnlimited.AutoCheckTimer_Elapsed;
            AutoCheckTimer.AutoReset = true;
            AutoCheckTimer.Enabled = true;

            U.Events.OnPlayerConnected += OnlinePlayerGroupManager.Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnlinePlayerGroupManager.Events_OnPlayerDisconnected;
            OnlineGroups = new List<ulong>();
            OnlinePlayers = new List<ulong>();

            Rocket.Core.Logging.Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded");
        }

        // todo:
        // track claim flags and generator placement
        // decay
        // healing
        // height limiter
        // offline/online raid prot
        // bypass perm for object placements
        // raid logs

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
        }

        private void StructurePlace(Structure structure, ItemStructureAsset asset, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            if (cfg.RoadPlaceBlocking.Enabled)
                RoadPlaceBlocking.Execute(asset.id, point, ref shouldAllow, owner);
        }

        private void BarricadeDamage(CSteamID instigatorSteamID, Transform barricadeTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            var barricade = BarricadeManager.FindBarricadeByRootTransform(barricadeTransform);
            var data = barricade.GetServersideData();
            Unbreakables.Execute(data.owner, data.group, ref shouldAllow, barricade.asset.itemName, barricade.asset.build, instigatorSteamID);
            if (barricadeTransform.parent != null && barricadeTransform.parent.TryGetComponent<InteractableVehicle>(out InteractableVehicle vehicle))
            {
                
            }
            else
            {
                
            }
        }

        private void BarricadePlace(Barricade barricade, ItemBarricadeAsset asset, Transform hit, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
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
            }
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

            AutoCheckTimer.Stop();
            AutoCheckTimer.Elapsed -= AutoToggleAndUnlimited.AutoCheckTimer_Elapsed;

            Rocket.Core.Logging.Logger.Log($"{Name} {Assembly.GetName().Version} has been unloaded");
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "PlacementBlocked", "You cannot place {0} on vehicles!" },
            { "NotYourVehicle", "That is not your vehicle!" },
            { "NoBuildHere", "You may not build on roads!" },
            { "MayNotDamage", "This {0} is protected from all damage!" }
        };
    }
}
