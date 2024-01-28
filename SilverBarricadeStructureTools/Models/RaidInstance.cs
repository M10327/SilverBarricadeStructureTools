using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SilverBarricadeStructureTools.Models
{
    public class RaidInstance
    {
        public RaidInstance(ulong owner, Raider raider, Vector3 position, string objectName, DateTime time)
        {
            OwnerId = owner;
            Raider = raider;
            Position = position;
            ObjectName = objectName;
            Time = time;
        }

        public ulong OwnerId { get; set; }
        public Raider Raider { get; set; }
        public Vector3 Position { get; set; }
        public string ObjectName { get; set; }
        public DateTime Time { get; set; }

        public string GetString()
        {
            return $"<t:{((DateTimeOffset)Time).ToUnixTimeSeconds()}:t> [{Raider.Name}](https://steamcommunity.com/profiles/{Raider.Id}) ({Raider.ItemName}) -> [{OwnerId}](https://steamcommunity.com/profiles/{OwnerId})'s {ObjectName} @ {Position}";
        }
    }

    public class Raider
    {
        public Raider(ulong id, string name, string itemname = "unarmed")
        {
            Id = id;
            Name = name;
            ItemName = itemname;
        }

        public ulong Id { get; set; }
        public string Name { get; set; }
        public string ItemName { get; set; }
    }
}
