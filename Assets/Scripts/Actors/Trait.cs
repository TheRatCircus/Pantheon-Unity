// Trait.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Actors
{
    public class Trait
    {
        [SerializeField] [ReadOnly] private string displayName;

        public delegate void TraitEffectDelegate(Actor actor);
        public TraitEffectDelegate OnGetTrait { get; private set; }
        public TraitEffectDelegate OnLoseTrait { get; private set; }

        public Trait(string displayName,
            TraitEffectDelegate onGet,
            TraitEffectDelegate onLose)
        {
            this.displayName = displayName;
            OnGetTrait = onGet;
            OnLoseTrait = onLose;
        }

        public override string ToString() => displayName;
    }
}