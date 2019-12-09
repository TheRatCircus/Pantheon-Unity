// EntityComponent.cs
// Jerome Martina

namespace Pantheon.Components
{
    [System.Serializable]
    public abstract class EntityComponent
    {
        public abstract EntityComponent Clone();
    }
}
