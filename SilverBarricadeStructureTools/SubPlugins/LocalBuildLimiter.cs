using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;

namespace SilverBarricadeStructureTools.SubPlugins
{
    public static class LocalBuildLimiter
    {
        public static void CheckStructures(ulong owner, Vector3 point, ref bool shouldAllow, ushort id)
        {
            var cfg = SBST.Instance.cfg.LocalBuildLimiter.Barricades;
            var c = cfg.Where(x => x.Ids.Contains(id)).FirstOrDefault();
            if (c == null) return;

            var checkrange = c.CheckRange * c.CheckRange;
            List<RegionCoordinate> regions = new List<RegionCoordinate>();
            Regions.getRegionsInRadius(point, checkrange, regions);

            List<Transform> BarricadeTransforms = new List<Transform>();
            StructureManager.getStructuresInRadius(point, checkrange, regions, BarricadeTransforms);

            int count = 0;

            foreach (var trans in BarricadeTransforms)
            {
                var bar = StructureManager.FindStructureByRootTransform(trans);
                if (bar == null) continue;
                if (c.Ids.Contains(bar.asset.id)) count++;
                if (count >= c.Limit)
                {
                    UnturnedChat.Say((CSteamID)owner, SBST.Instance.Translate("LocalBuildLimit", c.Limit, c.Name, c.CheckRange), SBST.Instance.MessageColor);
                    shouldAllow = false;
                    return;
                }
            }
        }

        public static void CheckBarricades(ulong owner, Vector3 point, ref bool shouldAllow, ushort id)
        {
            var cfg = SBST.Instance.cfg.LocalBuildLimiter.Barricades;
            var c = cfg.Where(x => x.Ids.Contains(id)).FirstOrDefault();
            if (c == null) return;

            var checkrange = c.CheckRange * c.CheckRange;
            List<RegionCoordinate> regions = new List<RegionCoordinate>();
            Regions.getRegionsInRadius(point, checkrange, regions);

            List<Transform> BarricadeTransforms = new List<Transform>();
            BarricadeManager.getBarricadesInRadius(point, checkrange, regions, BarricadeTransforms);

            int count = 0;

            foreach (var trans in BarricadeTransforms)
            {
                var bar = BarricadeManager.FindBarricadeByRootTransform(trans);
                if (bar == null) continue;
                if (c.Ids.Contains(bar.asset.id)) count++;
                if (count >= c.Limit)
                {
                    UnturnedChat.Say((CSteamID)owner, SBST.Instance.Translate("LocalBuildLimit", c.Limit, c.Name, c.CheckRange), SBST.Instance.MessageColor);
                    shouldAllow = false;
                    return;
                }
            }
        }
    }
}
