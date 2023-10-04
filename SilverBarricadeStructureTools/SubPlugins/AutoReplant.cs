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
    public static class AutoReplant
    {
        public static void Execute(InteractableFarm plant, SteamPlayer instigatorPlayer, ref bool shouldAllow)
        {
            if (plant.isPlant) return;
            if (!shouldAllow) return;
            UnturnedPlayer p = UnturnedPlayer.FromSteamPlayer(instigatorPlayer);
            if (p == null) return;
            if (p.Player.stance.stance != EPlayerStance.CROUCH) return;
            var bar = BarricadeManager.FindBarricadeByRootTransform(plant.transform);
            Rocket.Core.Logging.Logger.Log($"{plant.planted}, {plant.growth}, {plant.IsFullyGrown}");
            var plants = p.Inventory.search(plant.grow, false, true);
            if (plants.Count < 1) return;
            var page = plants[0].page;
            var index = p.Inventory.getIndex(page, plants[0].jar.x, plants[0].jar.y);
            p.Inventory.removeItem(page, index);
            
            var seedBarricadeToPlace = new Barricade(bar.asset);
            var newPlant = BarricadeManager.dropNonPlantedBarricade(seedBarricadeToPlace, plant.transform.position, plant.transform.rotation, bar.GetServersideData().owner, bar.GetServersideData().group);
            BarricadeManager.updateFarm(newPlant.transform, plant.planted, true);
        }
    }
}
