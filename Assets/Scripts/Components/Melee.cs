// Melee.cs
// Jerome Martina

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class Melee : EntityComponent
    {
       [UnityEngine.SerializeField] private Attack[] attacks = default;

        public Attack[] Attacks => attacks;

        public Melee(Attack[] attacks) => this.attacks = attacks;

        public override EntityComponent Clone() => new Melee(attacks);
    }
}
