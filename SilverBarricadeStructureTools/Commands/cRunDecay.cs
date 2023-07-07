using Rocket.API;
using Rocket.Unturned.Chat;
using SilverBarricadeStructureTools.SubPlugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBarricadeStructureTools.Commands
{
    public class cRunDecay : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "RunDecay";

        public string Help => "Runs one decay cycle. Mostly a debug command";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "rundecay" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Decay.StartDecays();
            UnturnedChat.Say("Running one decay cycle");
        }
    }
}
