// Ground.cs
// Jerome Martina

namespace Pantheon.ECS.Components
{
    /// <summary>
    /// An entity with a Ground component is walkable by creatures.
    /// </summary>
    [System.Serializable]
    public sealed class Ground : BaseComponent
    {
        public override BaseComponent Clone()
        {
            return new Ground();
        }
    }
}
