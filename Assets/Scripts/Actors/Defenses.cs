// Defenses.cs
// Jerome Martina

using Pantheon.Components;
using System;
using UnityEngine;

namespace Pantheon.Actors
{
    [Serializable]
    public sealed class Defenses
    {
        [SerializeField] private int armour;
        [SerializeField] private int evasion;

        // Resists run from -1.0 to 1.0
        [SerializeField] private float resistPhys = 0.0f; 
        [SerializeField] private float resistHeat;
        [SerializeField] private float resistCold;
        [SerializeField] private float resistDisease;
        [SerializeField] private float resistPoison;

        public int Armour { get => armour;
            set
            {
                armour = value;
                HUDDefenseChangeEvent?.Invoke(this);
            }
        }
        public int Evasion { get => evasion;
            set
            {
                evasion = value;
                HUDDefenseChangeEvent?.Invoke(this);
            }
        }

        public event Action<Defenses> HUDDefenseChangeEvent;

        /// <summary>
        /// Pass damage through resistance before applying to an actor.
        /// </summary>
        /// <param name="damage">Incoming damage before resistances.</param>
        /// <param name="damageType"></param>
        /// <returns>Incoming damage after resistances.</returns>
        public int Resist(int damage, DamageType damageType)
        {
            float res;
            switch (damageType)
            {
                case DamageType.Slashing:
                case DamageType.Piercing:
                case DamageType.Bludgeoning:
                    res = resistPhys;
                    break;
                default:
                    throw new Exception("No damage type given.");
            }
            res = 1.0f - res;
            return (int)(damage * res);
        }
    }
}
