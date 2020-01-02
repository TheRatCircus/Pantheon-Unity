// Binders.cs
// Jerome Martina

using Pantheon.Commands.NonActor;
using Pantheon.Components;
using Pantheon.Components.Statuses;
using Pantheon.Gen;
using System;
using System.Collections.Generic;
using Relic = Pantheon.Components.Relic;

namespace Pantheon.Serialization.Json
{
    /// <summary> Serialization binders for use with JSON.NET. </summary>
    public static class Binders
    {
        public static readonly SerializationBinder builder
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

        public static readonly SerializationBinder entity
            = new SerializationBinder
            {
                KnownTypes = new List<Type>()
                {
                    typeof(Actor),
                    typeof(AI),
                    typeof(Ammo),
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
                    typeof(Status),
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
                    // Statuses
                    typeof(Momentum),
                    // Misc
                    typeof(Talent)
                }
            };
    }
}
