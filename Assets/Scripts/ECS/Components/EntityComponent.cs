// EntityComponent.cs
// Jerome Martina

using Newtonsoft.Json;

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    public abstract class EntityComponent
    {
        [JsonIgnore] public int GUID { get; set; }

        public abstract EntityComponent Clone();
    }
}
