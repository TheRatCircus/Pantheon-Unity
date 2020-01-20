// Binders.cs
// Jerome Martina

using Pantheon.Commands.NonActor;
using Pantheon.Components;
using Pantheon.Components.Statuses;
using Pantheon.Content.Talents;
using Pantheon.Gen;
using System;
using System.Collections.Generic;
using Relic = Pantheon.Components.Relic;

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
                    typeof(Health),
                    typeof(Inventory),
                    typeof(Melee),
                    typeof(OnDamageTaken),
                    typeof(OnUse),
                    typeof(Relic),
                    typeof(Size),
                    typeof(Species),
                    typeof(Status),
                    typeof(Talents),
                    typeof(Weight),
                    typeof(Wield),
                    // AI utilities
                    typeof(AIUtility),
                    typeof(ApproachUtility),
                    typeof(AttackUtility),
                    typeof(FleeUtility),
                    // Commands
                    typeof(ExplodeCommand),
                    typeof(LineEffectCommand),
                    typeof(PointEffectCommand),
                    typeof(StatusCommand),
                    // Statuses
                    typeof(Momentum),
                    // Talents
                    typeof(Talent),
                    typeof(Punch)
                }
            };
    }
}
