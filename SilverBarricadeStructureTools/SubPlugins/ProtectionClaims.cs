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
    public class ProtectionClaims
    {
        public static void CheckProtected(ulong owner, Vector3 point, ref bool shouldAllow, ItemStructureAsset assetS, ItemBarricadeAsset assetB)
        {
            var cfg = SBST.Instance.cfg.ProtectionClaims;
            if (assetB != null)
            {
                if (assetB.build == EBuild.FARM)
                    return;
                var zz = cfg.IgnoreBuildTypes.ConvertAll(x => (EBuild)Enum.Parse(typeof(EBuild), x.ToUpper()));
                if (zz.Contains(assetB.build))
                    return;
            }
            
            var checkrange = 32 * 32;
            List<RegionCoordinate> regions = new List<RegionCoordinate>();
            Regions.getRegionsInRadius(point, checkrange, regions);

            List<Transform> BarricadeTransforms = new List<Transform>();
            BarricadeManager.getBarricadesInRadius(point, checkrange, regions, BarricadeTransforms);

            var flags = BarricadeTransforms
                .ConvertAll(x => BarricadeManager.FindBarricadeByRootTransform(x).asset)
                .Where(z => z != null && z.build == EBuild.CLAIM && cfg.Ids.Contains(z.id)).ToList();

            if (flags.Count > 0)
            {
                string name = "Unknown";
                if (assetB != null) name = assetB.itemName;
                else if (assetS != null) name = assetS.itemName;
                if (owner > 1000) UnturnedChat.Say((CSteamID)owner, SBST.Instance.Translate("MayNotDamage", name), SBST.Instance.MessageColor);
                shouldAllow = false;
            }
        }
    }
}
