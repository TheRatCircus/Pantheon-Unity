// Talent.cs
// Jerome Martina

using Pantheon.Commands;

namespace Pantheon
{
    public sealed class Talent
    {
        public NonActorCommand[] Commands { get; set; }

        public Talent(params NonActorCommand[] commands)
        {
            Commands = commands;
        }

        public CommandResult Cast(Entity evoker)
        {
            CommandResult result = CommandResult.Succeeded;

            foreach (NonActorCommand nac in Commands)
            {
                nac.Entity = evoker;
                CommandResult r = nac.Execute();

                if (r == CommandResult.InProgress)
                    result = CommandResult.InProgress;
                if (r == CommandResult.Cancelled)
                    result = CommandResult.Cancelled;
            }

            return result;
        }
    }
}