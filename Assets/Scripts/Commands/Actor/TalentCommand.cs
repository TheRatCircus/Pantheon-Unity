// TalentCommand.cs
// Jerome Martina

using Pantheon.Core;
using NewTalent = Pantheon.Content.Talents.Talent;

namespace Pantheon.Commands.Actor
{
    public sealed class TalentCommand : ActorCommand
    {
        private readonly NewTalent talent;

        public TalentCommand(Entity entity, NewTalent talent) : base(entity)
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
