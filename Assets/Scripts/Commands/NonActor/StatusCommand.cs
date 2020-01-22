// StatusCommand.cs
// Jerome Martina

using Newtonsoft.Json;
using Pantheon.Components.Entity;
using Pantheon.Components.Entity.Statuses;

namespace Pantheon.Commands.NonActor
{
    [System.Serializable]
    public sealed class StatusCommand : NonActorCommand, IEntityTargetedCommand
    {
        [JsonIgnore] public Entity Target { get; set; }
        public StatusDefinition Definition { get; set; }
        public int Duration { get; set; }
        public int Magnitude { get; set; }

        public StatusCommand(Entity applier, Entity target,
            StatusDefinition definition, int duration, int magnitude)
            : base(applier)
        {
            Target = target;
            Definition = definition;
            Duration = duration;
            Magnitude = magnitude;
        }

        public override CommandResult Execute()
        {
            if (!Target.TryGetComponent(out Status status))
                return CommandResult.Failed;

            StatusEffect effect = new StatusEffect(Definition, Duration, Magnitude);

            Status.ApplyStatus(Target, effect);
            return CommandResult.Succeeded;
        }
    }
}
