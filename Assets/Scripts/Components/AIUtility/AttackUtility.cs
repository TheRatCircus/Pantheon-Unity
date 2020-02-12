// AttackUtility.cs
// Jerome Martina

using Pantheon.Commands.Actor;
using System;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;
    using Talent = Pantheon.Talent;

    [Serializable]
    public sealed class AttackUtility : AIUtility
    {
        public int Aggression { get; private set; } = 50;

        private Talent talent;

        public AttackUtility(int aggression = 50) => Aggression = aggression;

        public override int Recalculate(Entity entity, AI ai)
        {
            if (ai.Target == null)
                return -1;
            int dist = entity.Level.Distance(entity.Cell, ai.Target.Cell);
            Talent[] talents = Talent.GetAllTalents(entity);

            if (talents.Length < 1)
                return -1;
            if (talents[0].Range < dist)
                return -1;

            // TODO: Use longest range talent available
            talent = talents[0];
            return Aggression;
        }

        public override ActorCommand Invoke(Entity entity, AI ai)
        {
            return new TalentCommand(entity, talent, ai.Target.Cell);
        }
    }
}
