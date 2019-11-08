// ComponentMessage.cs
// Jerome Martina

using Pantheon.ECS.Components;
using System;

namespace Pantheon.ECS.Messages
{
    public abstract class ComponentMessage
    {
        public Type Target { get; private set; }
        public BaseComponent Source { get; private set; }

        public ComponentMessage(BaseComponent source) => Source = source;

        public void SetTarget<T>() where T : BaseComponent
            => Target = typeof(T);
    }
}
