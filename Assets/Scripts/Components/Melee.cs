// Melee.cs
// Jerome Martina

namespace Pantheon.Components
{
    [System.Serializable]
    public sealed class Melee : EntityComponent
    {
       [UnityEngine.SerializeField] private MeleeAttack[] attacks = default;

        public MeleeAttack[] Attacks => attacks;

        public Melee(MeleeAttack[] attacks) => this.attacks = attacks;

        public override EntityComponent Clone(bool full) => new Melee(attacks);
    }
}
