// Weight.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Components
{
    /// <summary>
    /// Entity's weight in kilograms.
    /// </summary>
    public sealed class Weight : EntityComponent
    {
        [SerializeField] private int value;
        public int Value
        {
            get => value;
            set => this.value = Mathf.Clamp(value, 0, int.MaxValue);
        }

        public Weight(int value) => this.value = value;

        public override EntityComponent Clone(bool full) => new Weight(value);
    }
}