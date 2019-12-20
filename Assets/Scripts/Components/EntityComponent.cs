// EntityComponent.cs
// Jerome Martina

using System;

namespace Pantheon.Components
{
    [Serializable]
    public abstract class EntityComponent
    {
        public event Func<Entity> GetEntityEvent;
        public event Action<IComponentMessage> MessageEvent;

        public Entity Entity => GetEntityEvent.Invoke();
        protected void Message(IComponentMessage msg) => MessageEvent.Invoke(msg);
        public virtual void Receive(IComponentMessage msg) { } // Nothing by default

        public abstract EntityComponent Clone(bool full);
    }

    public interface IComponentMessage
    {
        EntityComponent source { get; }
    }

    public struct DamageEventMessage : IComponentMessage
    {
        public EntityComponent source { get; private set; }

        public DamageEventMessage(EntityComponent source) => this.source = source;
    }
}
