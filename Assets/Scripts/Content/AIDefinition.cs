// AIDefinition.cs
// Jerome Martina

using Pantheon.Components.Entity;

namespace Pantheon.Content
{
    public sealed class AIDefinition
    {
        public string ID { get; set; } = "DEFAULT_AI_DEF";
        public bool Peaceful { get; set; }
        public int SweetSpot { get; set; } = 1;
        public AIStrategy Strategy { get; set; }
        public AIUtility[] Utilities { get; set; }
        public byte Memory { get; set; } = 10;
    }
}
