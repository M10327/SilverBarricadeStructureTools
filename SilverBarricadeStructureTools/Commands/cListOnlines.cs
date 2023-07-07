using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBarricadeStructureTools.Commands
{
    public class cListOnlines : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "ListOnlines";

        public string Help => "List ids of online players and groups";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "listonlines" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedChat.Say(caller, $"Online Players: {string.Join(", ", SBST.Instance.OnlinePlayers)}");
            UnturnedChat.Say(caller, $"Online Groups: {string.Join(", ", SBST.Instance.OnlineGroups)}");
        }
    }
}
