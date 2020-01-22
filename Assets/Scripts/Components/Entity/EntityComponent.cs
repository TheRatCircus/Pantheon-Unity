// EntityComponent.cs
// Jerome Martina

using Newtonsoft.Json;
using System;

namespace Pantheon.Components.Entity
{
    using Entity = Pantheon.Entity;

    [Serializable]
    public abstract class EntityComponent
    {
        public event Func<Entity> GetEntityEvent;
        public event Action<IComponentMessage> MessageEvent;

        [JsonIgnore] public Entity Entity => GetEntityEvent.Invoke();
        protected void Message(IComponentMessage msg) => MessageEvent.Invoke(msg);
        public virtual void Receive(IComponentMessage msg) { } // Nothing by default

        public abstract EntityComponent Clone(bool full);
    }

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
