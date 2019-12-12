// AI.cs
// Jerome Martina

#define DEBUG_AI
#undef DEBUG_AI

using Newtonsoft.Json;

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    public sealed class AI : EntityComponent
    {
        [JsonIgnore] public Entity Target { get; set; }

        public override EntityComponent Clone() => new AI();
    }
}
