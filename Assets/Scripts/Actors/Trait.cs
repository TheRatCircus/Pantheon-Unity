// Trait.cs
// Jerome Martina

namespace Pantheon.Actors
{
    public class Trait
    {
        public string DisplayName { get; private set; }

        public delegate void TraitEffectDelegate(Actor actor);
        public TraitEffectDelegate OnGetTrait { get; private set; }
        public TraitEffectDelegate OnLoseTrait { get; private set; }

        public Trait(string displayName,
            TraitEffectDelegate onGet,
            TraitEffectDelegate onLose)
        {
            DisplayName = displayName;
            OnGetTrait = onGet;
            OnLoseTrait = onLose;
        }

        public override string ToString() => DisplayName;
    }
}