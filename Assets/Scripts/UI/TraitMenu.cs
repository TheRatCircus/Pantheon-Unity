// TraitMenu.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.UI;
using Pantheon.Actors;

namespace Pantheon.UI
{
    public class TraitMenu : MonoBehaviour
    {
        public Player Player { get; private set; }

        [SerializeField] private Text traitPointCounter;

        private void Start()
        {
            traitPointCounter.text = $"Trait Points: 0";
        }

        public void Initialize(Player player)
        {
            Player = player;
            Player.TraitPointChangeEvent += UpdateTraitPointCounter;
        }

        public void TryGetTrait(TraitStar traitStar)
        {
            if (Player.TraitPoints < 1)
                return;

            if (traitStar.Acquired)
                return;

            foreach (TraitStar ts in traitStar.Prereqs)
                if (!ts.Acquired)
                    return;

            if (!Traits._traits.TryGetValue(traitStar.TraitRef,
                out Trait trait))
                throw new System.Exception($"{traitStar.gameObject.name}" +
                    $" has no TraitRef.");

            Player.AddTrait(trait);
            Player.ChangeTraitPoints(-1);
            traitStar.Acquire();
            Core.GameLog.Send($"Trait acquired: {trait.DisplayName}!",
                Utils.Strings.TextColour.Green);
        }

        private void UpdateTraitPointCounter(int traitPoints)
        {
            traitPointCounter.text = $"Trait Points: {traitPoints}";
        }
    }
}
