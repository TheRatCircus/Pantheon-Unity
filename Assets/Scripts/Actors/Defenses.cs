// Defenses.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Actors
{
    [System.Serializable]
    public sealed class Defenses
    {
        [SerializeField] private int armour;
        [SerializeField] private int evasion;

        // Resists run from -1.0 to 1.0
        [SerializeField] private float resistPhys; 
        [SerializeField] private float resistHeat;
        [SerializeField] private float resistCold;
        [SerializeField] private float resistDisease;
        [SerializeField] private float resistPoison;

        public int Armour { get => armour; set => armour = value; }
        public int Evasion { get => evasion; set => evasion = value; }

        public event System.Action<Defenses> RecalculateEvent;

        public void Recalculate(Actor actor)
        {
            int ac = 0
                + actor.Species.Defenses.Armour
                + actor.Equipment.GetArmour();
            int ev = 0
                + actor.Species.Defenses.Evasion;

            armour = ac;
            evasion = ev;

            RecalculateEvent?.Invoke(this);
        }
    }
}
