// Binders.cs
// Jerome Martina

using Pantheon.Components;
using Pantheon.Gen;
using System;
using System.Collections.Generic;

namespace Pantheon.Serialization.Json
{
    /// <summary> Serialization binders for use with JSON.NET. </summary>
    public static class Binders
    {
        public static readonly SerializationBinder _builderStepBinder
            = new SerializationBinder
            {
                KnownTypes = new List<Type>()
                {
                    typeof(Fill),
                    typeof(RandomFill)
                }
            };

        public static readonly SerializationBinder _entityBinder
            = new SerializationBinder
            {
                KnownTypes = new List<Type>()
                {
                    typeof(Actor),
                    typeof(AI),
                    typeof(Ammo),
                    typeof(Health),
                    typeof(Inventory),
                    typeof(Melee),
                    typeof(OnDamageTaken),
                    typeof(OnUse),
                    typeof(Size),
                    typeof(Species),
                    typeof(Weight),
                    // AI strategies
                    typeof(DefaultStrategy),
                    typeof(SleepStrategy),
                    typeof(WanderStrategy),
                    typeof(ThrallFollowStrategy)
                }
            };
    }
}
