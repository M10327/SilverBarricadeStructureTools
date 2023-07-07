using HarmonyLib;
using JetBrains.Annotations;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBarricadeStructureTools
{
    [HarmonyPatch]
    internal static class Patches
    {
        public delegate void BarricadeDestroyed(BarricadeDrop drop);
        public static event BarricadeDestroyed? OnBarricadeDestroying;

        public delegate void StructureDestroyed(StructureDrop drop);
        public static event StructureDestroyed? OnStructureDestroying;
        private static Harmony PatcherInstance;
        internal static void PatchAll()
        {
            PatcherInstance = new Harmony("SilverBarricadeStructureTools");
            PatcherInstance.PatchAll();
        }
        internal static void UnpatchAll()
        {
            PatcherInstance.UnpatchAll("SilverBarricadeStructureTools");
        }

        [HarmonyPatch(typeof(BarricadeManager), nameof(BarricadeManager.ReceiveDestroyBarricade))]
        [HarmonyPrefix]
        private static void BarricadeDestroyedH(NetId netId)
        {
            var barricade = NetIdRegistry.Get<BarricadeDrop>(netId);
            if (barricade == null)
            {
                return;
            }

            OnBarricadeDestroying?.Invoke(barricade);
        }

        [HarmonyPatch(typeof(StructureManager), nameof(StructureManager.ReceiveDestroyStructure))]
        [HarmonyPrefix]
        private static void StructureDestroyedH(NetId netId)
        {
            var structure = NetIdRegistry.Get<StructureDrop>(netId);
            if (structure == null)
            {
                return;
            }

            OnStructureDestroying?.Invoke(structure);
        }
    }
}
