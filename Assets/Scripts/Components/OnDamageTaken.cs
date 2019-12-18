// OnDamageTaken.cs
// Jerome Martina

using Pantheon.Commands;

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class OnDamageTaken : EntityComponent
    {
        public NonActorCommand[] Commands { get; set; }

        public OnDamageTaken(params NonActorCommand[] commands) => Commands = commands;

        public override void Receive(IComponentMessage msg)
        {
            if (!(msg is DamageEventMessage))
                return;
            else
                foreach (NonActorCommand nac in Commands)
                    nac.Execute();
        }

        public override EntityComponent Clone(bool full) => new OnDamageTaken(Commands);
    }
}
