// AIRequestMessage.cs
// Jerome Martina

using Pantheon.ECS.Components;

namespace Pantheon.ECS.Messages
{
    public sealed class AIRequestMessage : ComponentMessage
    {
        public AIRequestMessage(BaseComponent source) : base(source) {}
    }
}
