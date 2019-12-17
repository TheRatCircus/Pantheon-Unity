// OnUse.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Core;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class OnUse : EntityComponent
    {
        private int useTime = TurnScheduler.TurnTime;
        private NonActorCommand[] commands;

        public OnUse(int useTime, params NonActorCommand[] commands)
        {
            this.commands = commands;
        }

        public int Invoke(Entity user)
        {
            foreach (NonActorCommand nac in commands)
            {
                nac.Entity = user;
                nac.Execute();
            }
            return useTime;
        }

        public override EntityComponent Clone() => new OnUse(useTime, commands);
    }
}
