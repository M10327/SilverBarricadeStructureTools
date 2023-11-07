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
            var cfg = SBST.Instance.cfg.LocalBuildLimiter.Structures;
            foreach (var conf in cfg)
            {
                if (!conf.Ids.Contains(id)) continue;
                var checkrange = conf.CheckRange * conf.CheckRange;
                List<RegionCoordinate> regions = new List<RegionCoordinate>();
                Regions.getRegionsInRadius(point, checkrange, regions);

                List<Transform> structureTransforms = new List<Transform>();
                StructureManager.getStructuresInRadius(point, checkrange, regions, structureTransforms);

                int count = 0;
                foreach (var trans in structureTransforms)
                {
                    var bar = StructureManager.FindStructureByRootTransform(trans);
                    if (bar == null) continue;
                    if (conf.Ids.Contains(id)) count++;
                    if (count >= conf.Limit)
                    {
                        UnturnedChat.Say((CSteamID)owner, SBST.Instance.Translate("LocalBuildLimit", conf.Limit, conf.Name, conf.CheckRange), SBST.Instance.MessageColor);
                        shouldAllow = false;
                        return;
                    }
                }
            }
        }

        public static void CheckBarricades(ulong owner, Vector3 point, ref bool shouldAllow, ushort id)
        {
            var cfg = SBST.Instance.cfg.LocalBuildLimiter.Barricades;
            foreach (var conf in cfg)
            {
                if (!conf.Ids.Contains(id)) continue;
                var checkrange = conf.CheckRange * conf.CheckRange;
                List<RegionCoordinate> regions = new List<RegionCoordinate>();
                Regions.getRegionsInRadius(point, checkrange, regions);

                List<Transform> BarricadeTransforms = new List<Transform>();
                BarricadeManager.getBarricadesInRadius(point, checkrange, regions, BarricadeTransforms);

                int count = 0;
                foreach(var trans in BarricadeTransforms)
                {
                    var bar = BarricadeManager.FindBarricadeByRootTransform(trans);
                    if (bar == null) continue;
                    if (conf.Ids.Contains(id)) count++;
                    if (count >= conf.Limit)
                    {
                        UnturnedChat.Say((CSteamID)owner, SBST.Instance.Translate("LocalBuildLimit", conf.Limit, conf.Name, conf.CheckRange), SBST.Instance.MessageColor);
                        shouldAllow = false;
                        return;
                    }
                }
            }
        }
    }
}
