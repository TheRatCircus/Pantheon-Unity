// Attack.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    [System.Serializable]
    public class Attack
    {
        [SerializeField] private Damage[] damages = default;
        [SerializeField] private int accuracy = -1; // 0...100
        [SerializeField] private int time = -1;

        public Damage[] Damages => damages;
        public int Accuracy => accuracy;
        public int Time => time;

        public Attack(Damage[] damages, int accuracy, int time)
        {
            this.damages = damages;
            this.accuracy = accuracy;
            this.time = time;
        }
    }

    [System.Serializable]
    public sealed class MeleeAttack : Attack
    {
        [SerializeField] private int range = 1;
        public int Range => range;

        public MeleeAttack(Damage[] damages, int accuracy, int time)
            : base(damages, accuracy, time) { }
    }

    [System.Serializable]
    public sealed class RangedAttack : Attack
    {
        [SerializeField] private int sweetSpot = 5;
        public int SweetSpot => sweetSpot;

        public RangedAttack(Damage[] damages, int accuracy, int time)
            : base(damages, accuracy, time) { }
    }
}
