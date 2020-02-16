// AttackUtility.cs
// Jerome Martina

using Pantheon.Commands.Actor;
using Pantheon.Utils;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;
    using Talent = Pantheon.Talent;

    public sealed class AttackUtility : AIUtility
    {
        public int Aggression { get; private set; } = 50;

        public AttackUtility(int aggression = 50) => Aggression = aggression;

        public override int Recalculate(Entity entity, AI ai)
        {
            if (!ai.Alerted)
                return -1;

            int dist = Helpers.Distance(entity.Cell, Locator.Player.Entity.Cell);
            Talent[] talents = Talent.GetAllTalents(entity);

            if (talents.Length < 1)
                return -1;
            if (talents[0].Range < dist)
                return -1;

            // TODO: Use longest range talent available
            return Aggression;
        }

        public override ActorCommand Invoke(Entity entity, AI ai)
        {
            Talent[] talents = Talent.GetAllTalents(entity);
            return new TalentCommand(entity, talents[0], Locator.Player.Entity.Cell);
        }
    }
}
