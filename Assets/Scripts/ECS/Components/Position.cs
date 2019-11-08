// Position.cs
// Jerome Martina

namespace Pantheon.ECS.Components
{
    public sealed class Position : BaseComponent
    {
        public Level Level { get; set; }
        public Cell Cell { get; set; }
    }
}
