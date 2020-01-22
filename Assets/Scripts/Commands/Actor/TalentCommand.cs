// TalentCommand.cs
// Jerome Martina

using Pantheon.World;

namespace Pantheon.Commands.Actor
{
    public sealed class TalentCommand : ActorCommand
    {
        private readonly Talent talent;
        private readonly Cell target;

        public TalentCommand(Entity entity, Talent talent, Cell target)
            : base(entity)
        {
            this.talent = talent;
            this.target = target;
        }

        public override CommandResult Execute(out int cost)
        {
            cost = talent.Time;
            return talent.Cast(Entity, target);
        }
    }
}
