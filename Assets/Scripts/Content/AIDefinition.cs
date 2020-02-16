// AIDefinition.cs
// Jerome Martina

using Pantheon.Components.Entity;

namespace Pantheon.Content
{
    public sealed class AIDefinition
    {
        public string ID { get; set; }
        public bool Peaceful { get; set; }
        public AIStrategy Strategy { get; set; }
        public AIUtility[] Utilities { get; set; }
    }
}
