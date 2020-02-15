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
        public event Action<IComponentMessage> MessageEvent;

        [JsonIgnore] public Entity Entity { get; set; }
        protected void Message(IComponentMessage msg) => MessageEvent.Invoke(msg);
        public virtual void Receive(IComponentMessage msg) { } // Nothing by default

        public abstract EntityComponent Clone(bool full);
    }
}
