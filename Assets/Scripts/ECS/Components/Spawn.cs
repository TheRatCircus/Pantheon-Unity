// Spawn.cs
// Jerome Martina

namespace Pantheon.ECS.Components
{
    /// <summary>
    /// Allows any entity to create more entities 
    /// by informing it of the entity factory.
    /// </summary>
    public sealed class Spawn : BaseComponent
    {
        public EntityFactory Factory { get; private set; }
    }
}
