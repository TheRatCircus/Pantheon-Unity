// Attack.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// An attack feasible by a Melee or Ranged component.
    /// </summary>
    [System.Serializable]
    public sealed class Attack
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
}
