using Rocket.API;
using SilverBarricadeStructureTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBarricadeStructureTools
{
    public class Config : IRocketPluginConfiguration
    {
        public string MessageColor;
        public bool BypassPlacementsAllow;
        public string BypassPlacementsPerm;
        public bool BlockBuildingOnOthersVehicles;
        public bool PrintPlacementInfoToConsole;
        public mRaidLogs raidLogs;
        public mVehicleBarricadeBlacklist VehicleBarricadeBlacklist;
        public mRoadPlaceBlocking RoadPlaceBlocking;
        public List<ulong> UnbreakablesIds;
        public mAutoToggleAndUnlimited AutoToggleAndUnlimited;
        public mHeightLimiter HeightLimiter;
        public mDecay Decay;
        public mOfflineRaidProt OfflineRaidProt;
        public mVehicleNoPlaceOn VehicleNoPlaceOn;
        public mBuildableRepairDelay BuildableRepairDelay;
        public bool AutoReplantEnabled;
        public mVehicleBuildCap VehicleBuildCap;
        public mLootProtect LootProtect;
        public mLocalBuildLimiter LocalBuildLimiter;
        public mProtectionClaims ProtectionClaims;
        public List<byte> BlockHordesNavList;
        public void LoadDefaults()
        {
            MessageColor = "ffff00";
            BypassPlacementsAllow = false;
            BypassPlacementsPerm = "silverbar.bypassplacements";
            BlockBuildingOnOthersVehicles = true;
            PrintPlacementInfoToConsole = false;
            raidLogs = new mRaidLogs()
            {
                WebhookUrl = "Discord url here",
                Name = "Raid Logs",
                IconUrl = "https://i.imgur.com/xOO7Acd.png",
                RaidsPerEmbed = 8,
                MaxSecondsBetweenPosts = 30,
                IgnoreWeaponIds = new List<ushort>() { 25836 },
                IgnorePlaceableIds = new List<ushort>() { 1241, 1393, 1691, 26019, 25880, 26259, 26260 }
            };
            VehicleBarricadeBlacklist = new mVehicleBarricadeBlacklist()
            {
                Enabled = true,
                Barricades = new List<ushort>() { 288, 289, 290, 291, 292, 293, 294, 295, 1243, 1309, 1310, 1311, 1312, 1313, 1314, 17623, 17624, 17625, 17626, 17627, 17628, 17629, 17630, 17631, 17632, 17633, 17634, 17635, 17636, 9821 },
            };
            RoadPlaceBlocking = new mRoadPlaceBlocking()
            {
                Enabled = true,
                MinHeightAboveRoad = 20,
                AllowedIds = new List<ulong>() { 1691, 25880 }
            };
            UnbreakablesIds = new List<ulong>() { 69, 103582791472243867 };
            AutoToggleAndUnlimited = new mAutoToggleAndUnlimited()
            {
                Enabled = true,
                SecondsBetweenChecks = 60,
                Generators = new List<ulong>() { 69, 103582791472243867 },
                Lights = new List<ulong>() { 69, 103582791472243867 },
                Oxygenators = new List<ulong>() { 69, 103582791472243867 },
                Fires = new List<ulong>() { 69, 103582791472243867 }
            };
            HeightLimiter = new mHeightLimiter()
            {
                Enabled = true,
                MaxHeight = 175,
                UseDynamicHeight = true,
                DynamicHeight = 125,
                EnabledHeightLimitedCommands = true,
                CommandsMinYToBlock = 100,
                BlockedCommands = new List<string>()
                {
                    "tpa",
                    "garageadd",
                    "gadd",
                    "ga"
                },
                CustomHeights = new List<CustomHeightObject>()
                {
                    new CustomHeightObject()
                    {
                        ID = 1691,
                        MaxHeight = 80,
                        DynamicHeight = 20
                    }
                }
            };
            Decay = new mDecay()
            {
                Enabled = true,
                DamageWhileOwnerOnline = false,
                DamagePercent = 1,
                HealPercent = 4,
                HoursTilFullDecay = 72,
                DecayIgnoreOwnerGroupIds = new List<ulong>() { 69 }
            };
            OfflineRaidProt = new mOfflineRaidProt()
            {
                Enabled = true,
                OnlineMulti = 1.0f,
                OfflineMulti = 0.5f
            };
            VehicleNoPlaceOn = new mVehicleNoPlaceOn()
            {
                Enabled = true,
                VehicleIDs = new List<ushort>() { 26108 }
            };
            BuildableRepairDelay = new mBuildableRepairDelay()
            {
                Enabled = true,
                RepairDelaySeconds = 10,
                UseChat = false,
                UseUI = true,
                UiId = 28001,
            };
            AutoReplantEnabled = true;
            VehicleBuildCap = new mVehicleBuildCap()
            {
                Enabled = true,
                BuildCaps = new List<VehicleBuildCapObject>()
                {
                    new VehicleBuildCapObject()
                    {
                        Name = "Sentries",
                        Ids = new List<ushort>() { 1244, 1372, 1373 },
                        MaxAllowed = 5
                    }
                }
            };
            LootProtect = new mLootProtect()
            {
                Enabled = true,
                AllowAllVulnerable = true,
                Resize = -15,
                Height = 900,
                AllowedIds = new List<ushort>() { 1070 },
                BlockedIds = new List<ushort>() { 288, 289, 290, 291, 292, 293, 294, 295 }
            };
            LocalBuildLimiter = new mLocalBuildLimiter()
            {
                Enabled = true,
                Barricades = new List<BuildLimitObject>()
                {
                    new BuildLimitObject()
                    {
                        CheckRange = 32,
                        Name = "Claimflags",
                        Limit = 10,
                        Ids = new List<ushort>() { 1158 }
                    }
                },
                Structures = null
            };
            ProtectionClaims = new mProtectionClaims()
            {
                Enabled = true,
                IgnoreBuildTypes = new List<string>() { "DOOR", "GATE", "HATCH", "SENTRY", "SENTRY_FREEFORM", "STORAGE", "STORAGE_WALL" },
                Ids = new List<ushort>() { 1158 }
            };
            BlockHordesNavList = new List<byte>() { 0 }; 
        }
    }
}
