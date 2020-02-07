// Binders.cs
// Jerome Martina

using Pantheon.Commands.NonActor;
using Pantheon.Components.Entity;
using Pantheon.Components.Entity.Statuses;
using Pantheon.Components.Talent;
using Pantheon.Gen;
using System;
using System.Collections.Generic;
using Relic = Pantheon.Components.Entity.Relic;

namespace Pantheon.Serialization.Json
{
    /// <summary> Serialization binders for use with JSON.NET. </summary>
    public static class Binders
    {
        public static readonly SerializationBinder _builder
            = new SerializationBinder
            {
                KnownTypes = new List<Type>()
                {
                    // Steps
                    typeof(BinarySpacePartition),
                    typeof(CellularAutomata),
                    typeof(Fill),
                    typeof(RandomFill),
                }
            };

        public static readonly SerializationBinder _entity
            = new SerializationBinder
            {
                KnownTypes = new List<Type>()
                {
                    typeof(Actor),
                    typeof(AI),
                    typeof(Ammo),
                    typeof(Body),
                    typeof(Corpse),
                    typeof(Evocable),
                    typeof(Health),
                    typeof(Inventory),
                    typeof(Melee),
                    typeof(OnDamageTaken),
                    typeof(OnUse),
                    typeof(Relic),
                    typeof(Size),
                    typeof(Species),
                    typeof(Splat),
                    typeof(Status),
                    typeof(Talented),
                    typeof(Weight),
                    typeof(Wield),
                    // Commands
                    typeof(ExplodeCommand),
                    typeof(LineEffectCommand),
                    typeof(PointEffectCommand),
                    typeof(StatusCommand),
                    // AI strategies
                    typeof(DefaultStrategy),
                    typeof(SleepStrategy),
                    typeof(WanderStrategy),
                    typeof(ThrallFollowStrategy),
                    // AI utilities
                    typeof(AIUtility),
                    typeof(ApproachUtility),
                    typeof(AttackUtility),
                    typeof(FleeUtility),
                    // Statuses
                    // Talent
                    typeof(Talent),
                    typeof(AdjacentAttack),
                    typeof(CircularAttack),
                    typeof(TossTalent)
                }
            };
    }
}
