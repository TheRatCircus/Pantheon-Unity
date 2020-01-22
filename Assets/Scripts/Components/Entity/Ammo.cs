// Ammo.cs
// Jerome Martina

using Pantheon.Commands;
using Pantheon.Commands.NonActor;

namespace Pantheon.Components.Entity
{
    public enum AmmoType
    {
        Arrows,
        Bolts,
        Cartridges,
        Shot
    }

    [System.Serializable]
    public sealed class Ammo : EntityComponent
    {
        public AmmoType Type { get; set; }
        public Damage[] Damages { get; set; }
        public NonActorCommand OnLandCommand { get; set; }
        public IEntityTargetedCommand OnHitCommand { get; set; }

        public Ammo() { }

        public override EntityComponent Clone(bool full)
        {
            throw new System.NotImplementedException();
        }
    }
}
