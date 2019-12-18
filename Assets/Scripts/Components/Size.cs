// Size.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Components
{
    /// <summary>
    /// Entity's displacement of volume.
    /// </summary>
    public sealed class Size : EntityComponent
    {
        public const int _jewellery = 0;
        public const int _dagger = 1;
        public const int _broadsword = 2;
        public const int _longsword = 3;
        public const int _polearm = 4;
        public const int _dog = 5;
        public const int _human = 6;
        public const int _horse = 7;
        public const int _rhino = 8;
        public const int _elephant = 9;
        public const int _maxValue = 10; // Whole cell displaced

        [SerializeField] private int value;
        public int Value
        {
            get => value;
            set
            {
                this.value = Mathf.Clamp(value, 0, _maxValue);
            }
        }

        public Size(int value) => this.value = value;

        public override EntityComponent Clone(bool full) => new Size(value);
    }
}
