// Health.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    public sealed class Health : BaseComponent
    {
        [SerializeField] private int health;
        [SerializeField] private int maxHealth = -1;
        [SerializeField] private int regenRate = -1; // Time to regen 1 HP
        [SerializeField] private int regenProgress = 0;
    }
}
