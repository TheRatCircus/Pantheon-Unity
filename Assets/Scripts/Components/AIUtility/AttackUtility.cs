// AI.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

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

        public AttackUtility(int aggression = 50) => Aggression = aggression;

        public override int Recalculate(Entity entity, AI ai)
        {
            if (ai.Target == null)
                return -1;
            int dist = entity.Level.Distance(entity.Cell, ai.Target.Cell);
            Talent[] talents = Talent.GetAllTalents(entity);
            if (talents[0].Range < dist)
                return -1;

            return Aggression;
        }

        public override ActorCommand Invoke(Entity entity, AI ai)
        {
            return new TalentCommand(
                entity, 
                entity.GetComponent<Talented>().Talents[0],
                ai.Target.Cell);
        }
    }
}