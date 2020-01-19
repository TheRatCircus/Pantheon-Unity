// TalentCommand.cs
// Jerome Martina

using Pantheon.Content.Talents;
using Pantheon.Core;

namespace Pantheon.Commands.Actor
{
    public sealed class TalentCommand : ActorCommand
    {
        private readonly Talent talent;

        public TalentCommand(Entity entity, Talent talent) : base(entity)
        {
            this.talent = talent;
        }

        public override CommandResult Execute(out int cost)
        {
            cost = TurnScheduler.TurnTime;
            return talent.Cast(Entity);
        }
    }
}
