// ComponentMessage.cs
// Jerome Martina

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    public interface IComponentMessage
    {
        EntityComponent Source { get; }
    }

    public struct DamageEventMessage : IComponentMessage
    {
        public EntityComponent Source { get; private set; }
        public Entity Damager { get; private set; }

        public DamageEventMessage(EntityComponent source, Entity damager)
        {
            Source = source;
            Damager = damager;
        }
    }
}
