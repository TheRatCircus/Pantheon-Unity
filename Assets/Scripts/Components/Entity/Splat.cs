// Splat.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Components.Entity
{
    [System.Serializable]
    public sealed class Splat : EntityComponent
    {
        public AudioClip Sound { get; set; }

        public override EntityComponent Clone(bool full)
        {
            return new Splat() { Sound = Sound };
        }
    }
}
