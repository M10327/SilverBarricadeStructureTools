using Rocket.Core.Utils;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBarricadeStructureTools.SubPlugins
{
    public static class AutoToggleAndUnlimited
    {
        public static void AutoCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (SBST.Instance.cfg.AutoToggleAndUnlimited.Enabled)
            {
                TaskDispatcher.QueueOnMainThread(() => Execute());
            }
        }

        public static async void Execute()
        {
            if (!Level.isLoaded) return;
            await Task.Delay(1);
            foreach (var region in BarricadeManager.BarricadeRegions)
            {
                foreach (var x in region.drops)
                {
                    try
                    {
                        if (x.interactable == null) continue;
                        var owner = x.GetServersideData().owner;
                        var group = x.GetServersideData().group;
                        if (x.interactable != null)
                        {
                            if (x.interactable is InteractableGenerator gen && (SBST.Instance.cfg.AutoToggleAndUnlimited.Generators.Contains(owner) || SBST.Instance.cfg.AutoToggleAndUnlimited.Generators.Contains(group)))
                            {
                                if (gen.fuel < gen.capacity)
                                {
                                    BarricadeManager.sendFuel(x.model.transform, gen.capacity);
                                }
                                if (!gen.isPowered)
                                {
                                    BarricadeManager.ServerSetGeneratorPowered(gen, true);
                                }
                            }
                            if (x.interactable is InteractableSpot spot && (SBST.Instance.cfg.AutoToggleAndUnlimited.Lights.Contains(owner) || SBST.Instance.cfg.AutoToggleAndUnlimited.Lights.Contains(group)))
                            {
                                if (!spot.isPowered)
                                {
                                    BarricadeManager.ServerSetSpotPowered(spot, true);
                                }
                            }
                            if (x.interactable is InteractableOxygenator oxy && (SBST.Instance.cfg.AutoToggleAndUnlimited.Oxygenators.Contains(owner) || SBST.Instance.cfg.AutoToggleAndUnlimited.Oxygenators.Contains(group)))
                            {
                                if (!oxy.isPowered)
                                {
                                    BarricadeManager.ServerSetOxygenatorPowered(oxy, true);
                                }
                            }
                            if (x.interactable is InteractableFire fire && (SBST.Instance.cfg.AutoToggleAndUnlimited.Fires.Contains(owner) || SBST.Instance.cfg.AutoToggleAndUnlimited.Fires.Contains(group)))
                            {
                                if (!fire.isLit)
                                {
                                    BarricadeManager.ServerSetFireLit(fire, true);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Rocket.Core.Logging.Logger.LogError($"Caught Exception: {ex}");
                    }
                }
            }
        }
    }
}
