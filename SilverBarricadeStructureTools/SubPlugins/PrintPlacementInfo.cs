using Rocket.Core.Logging;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace SilverBarricadeStructureTools.SubPlugins
{
    public static class PrintPlacementInfo
    {
        public static void Structure(ItemStructureAsset asset, Vector3 point, float angle_x, float angle_y, float angle_z, ulong owner, ulong group)
        {
            Common(point, angle_x, angle_y, angle_z, owner, group, asset.itemName);
        }

        public static void Barricade(ItemBarricadeAsset asset, Vector3 point, float angle_x, float angle_y, float angle_z, ulong owner, ulong group, bool onVehicle, InteractableVehicle vehicle)
        {
            Common(point, angle_x, angle_y, angle_z, owner, group, asset.itemName);
            if (onVehicle)
            {
                Logger.Log($"Vehicle: {vehicle.asset.FriendlyName}\n" +
                    $"Vehicle Position: {vehicle.transform.position}");
            }
        }

        public static void Common(Vector3 pos, float angle_x, float angle_y, float angle_z, ulong owner, ulong group, string name)
        {
            
            Logger.Log($"{name}\n" +
                    $"Position: {pos}\n" +
                    $"Angles: {angle_x}, {angle_y}, {angle_z}\n" +
                    $"Owner: {owner}\n" +
                    $"Group: {group}");
            if (LevelNavigation.tryGetBounds(pos, out byte nav))
                Logger.Log($"Navmesh ID {nav}\n");
        }
    }
}
