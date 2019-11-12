// BaseComponent.cs
// Jerome Martina

using Pantheon.ECS.Messages;

namespace Pantheon.ECS.Components
{
    public abstract class BaseComponent
    {
        public int GUID { get; private set; } = -1;
        public bool Enabled { get; set; } // Do systems update this component?
        public event System.Action<ComponentMessage> MessageEvent;

        public void AssignToEntity(Entity e)
        {
            GUID = e.GUID;
        }

        public void Message<T>(ComponentMessage msg)
            where T : BaseComponent
        {
            msg.SetTarget<T>();
            MessageEvent?.Invoke(msg);
        }

        public virtual void Receive(ComponentMessage msg)
        {
            throw new System.NotImplementedException(
                "This component does not support messaging.");
        }

        public abstract BaseComponent Clone();
    }
}
