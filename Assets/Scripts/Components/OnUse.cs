// OnUse.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Core;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class OnUse : EntityComponent
    {
        public int UseTime { get; private set; } = TurnScheduler.TurnTime;
        private NonActorCommand[] commands;

        public OnUse(int useTime, params NonActorCommand[] commands)
        {
            this.commands = commands;
        }

        public CommandResult Invoke(Entity user)
        {
            CommandResult result = CommandResult.Succeeded;

            foreach (NonActorCommand nac in commands)
            {
                nac.Entity = user;
                CommandResult r = nac.Execute();

                if (r == CommandResult.InProgress)
                    result = CommandResult.InProgress;
                if (r == CommandResult.Cancelled)
                    result = CommandResult.Cancelled;
            }

            return result;
        }

        public override EntityComponent Clone() => new OnUse(UseTime, commands);
    }
}
