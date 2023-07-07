using Rocket.Core.Utils;
using SDG.Unturned;
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
    public static class Decay
    {
        public static void DecayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (SBST.Instance.cfg.AutoToggleAndUnlimited.Enabled)
            {
                TaskDispatcher.QueueOnMainThread(() => StartDecays());
            }
        }

        public static async void StartDecays()
        {
            await Task.Delay(1);
            ulong startime = (ulong)((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
            (var claims, var healingClaims) = await FindClaimFlagsAndGenerators();
            await DamageBarricades(claims, healingClaims);
            await DamageStructures(claims, healingClaims);
            ulong endtime = (ulong)((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
            Rocket.Core.Logging.Logger.Log("Finished structure checking and damaging in " + (endtime - startime) + "ms");
        }

        private static async Task<(List<BarricadeStore> claims, List<BarricadeStore> healingClaims)> FindClaimFlagsAndGenerators()
        {
            await Task.Delay(1);
            List<BarricadeStore> claims = new List<BarricadeStore>();
            List<BarricadeStore> gens = new List<BarricadeStore>();
            List<BarricadeStore> healingClaims = new List<BarricadeStore>();
            // grab locs of claim flags and generators 
            foreach (var region in BarricadeManager.BarricadeRegions)
            {
                foreach (var x in region.drops)
                {
                    if (x.interactable != null)
                    {
                        if (x.interactable is InteractableClaim)
                        {
                            claims.Add(new BarricadeStore() { Pos = x.GetServersideData().point, Owner = x.GetServersideData().owner, Group = x.GetServersideData().group, Transform = x.model.transform });
                        }
                        else if (x.interactable is InteractableGenerator g)
                        {
                            if (g.isPowered && g.fuel > 1)
                            {
                                gens.Add(new BarricadeStore() { Pos = x.GetServersideData().point, Owner = x.GetServersideData().owner, Group = x.GetServersideData().group, Transform = x.model.transform });
                            }
                        }
                    }
                }
            }
            // checks claim flags for if there is a gen nearby
            foreach (var barricade in claims)
            {
                var cl = BarricadeManager.FindBarricadeByRootTransform(barricade.Transform);
                foreach (var gen in gens)
                {
                    if (Vector3.Distance(cl.GetServersideData().point, gen.Pos) <= 32)
                    {
                        healingClaims.Add(new BarricadeStore() { Pos = cl.GetServersideData().point, Owner = cl.GetServersideData().owner, Group = cl.GetServersideData().group, Transform = cl.model.transform});
                        break;
                    }
                }
            }
            return (claims, healingClaims);
        }

        public static DecayOutcome CheckHurt(List<BarricadeStore> claims, List<BarricadeStore> healClaims, DecayBuildableInfo data)
        {
            bool nearFlag = false;            
            foreach (var cf in claims) // check if near flag
            {
                if (Vector3.Distance(data.Point, cf.Pos) <= 32)
                {
                    if (cf.Owner == data.Owner || (cf.Group == data.Group && cf.Group != 0))
                    {
                        nearFlag = true;
                        break;
                    }
                }
            } 
            if (!nearFlag) // not near a flag, determine if should hurt or do nothing
            {
                if ((!SBST.Instance.OnlinePlayers.Contains(data.Owner) || SBST.Instance.cfg.Decay.DamageWhileOwnerOnline)){
                    if (SBST.Instance.cfg.Decay.DecayIgnoreOwnerGroupIds.Contains(data.Owner) || data.IsFarm)
                    {
                        return DecayOutcome.neither;
                    }
                    else
                    {
                        return DecayOutcome.hurt;
                    }
                }
            }
            else // near a flag. determine if it should heal instead
            {
                if (data.HealthCurrent < data.HealthMax)
                {
                    foreach (var cf in healClaims)
                    {
                        if (Vector3.Distance(data.Point, cf.Pos) <= 32)
                        {
                            if (cf.Owner == data.Owner || (cf.Group == data.Group && cf.Group != 0))
                            {
                                return DecayOutcome.heal;
                            }
                        }
                    }
                }
            }
            return DecayOutcome.neither;
        }

        private static async Task DamageBarricades(List<BarricadeStore> claims, List<BarricadeStore> healClaims)
        {
            await Task.Delay(1);
            foreach (var region in BarricadeManager.regions)
            {
                foreach (var d in region.drops.ToArray())
                {
                    var data = d.GetServersideData();
                    var outcome = CheckHurt(claims, healClaims, new DecayBuildableInfo(d.asset.build == EBuild.FARM, data.point, data.owner, data.group, data.barricade.health, d.asset.health));
                    if (outcome == DecayOutcome.hurt)
                    {
                        float dmg = d.asset.health * (SBST.Instance.cfg.Decay.DamagePercent / 100);
                        if (dmg < 1) dmg = 1;
                        BarricadeManager.damage(d.model, dmg, 1, false, default(CSteamID), EDamageOrigin.Unknown);
                    }
                    else if (outcome == DecayOutcome.heal)
                    {
                        float repairAmount = (float)d.asset.health * (SBST.Instance.cfg.Decay.HealPercent / 100);
                        if (repairAmount < 1) repairAmount = 1;
                        BarricadeManager.repair(d.model.transform, repairAmount, 1);
                    }
                }
            }
        }

        private static async Task DamageStructures(List<BarricadeStore> claims, List<BarricadeStore> healClaims)
        {
            await Task.Delay(1);
            foreach (var region in StructureManager.regions)
            {
                foreach (var d in region.drops.ToArray())
                {
                    var data = d.GetServersideData();
                    var outcome = CheckHurt(claims, healClaims, new DecayBuildableInfo(false, data.point, data.owner, data.group, data.structure.health, d.asset.health));
                    if (outcome == DecayOutcome.hurt)
                    {
                        float dmg = d.asset.health * (SBST.Instance.cfg.Decay.DamagePercent / 100);
                        if (dmg < 1) dmg = 1;
                        StructureManager.damage(d.model, new Vector3(0, 0, 0), dmg, 1, false, default(CSteamID), EDamageOrigin.Unknown);
                    }
                    else if (outcome == DecayOutcome.heal)
                    {
                        float repairAmount = (float)d.asset.health * (SBST.Instance.cfg.Decay.HealPercent / 100);
                        if (repairAmount < 1) repairAmount = 1;
                        StructureManager.repair(d.model.transform, repairAmount, 1);
                    }
                }
            }
        }

    }
}
