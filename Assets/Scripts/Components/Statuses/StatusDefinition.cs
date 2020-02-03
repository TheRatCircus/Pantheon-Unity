// StatusDefinition.cs
// Jerome Martina

using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Pantheon.Components.Entity.Statuses
{
    using Entity = Pantheon.Entity;

    public abstract class StatusDefinition
    {
        public abstract string ID { get; }
        public abstract string Name { get; }
        public abstract Sprite Icon { get; }
        public abstract bool HasMagnitude { get; }

        public abstract void Apply(Entity entity);
        public abstract void PerTurn(Entity entity);
        public abstract void Expire(Entity entity);

        private static readonly Dictionary<string, StatusDefinition> readonlyStatuses
            = new Dictionary<string, StatusDefinition>
        {
        };

        public static readonly ReadOnlyDictionary<string, StatusDefinition> statuses
            = new ReadOnlyDictionary<string, StatusDefinition>(readonlyStatuses);
    }
}
