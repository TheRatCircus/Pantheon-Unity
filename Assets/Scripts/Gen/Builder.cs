// Builder.cs
// Jerome Martina

using Pantheon.Content;
using Pantheon.Utils;
using UnityEngine;

namespace Pantheon.Gen
{
    /// <summary>
    /// An object used to generate a level upon request.
    /// </summary>
    public sealed class Builder
    {
        public string ID { get; set; }
        public string LevelID { get; set; }
        public string DisplayName { get; set; }
        public Vector3Int Position { get; set; }
        public Vector2Int Size { get; set; }
        public TerrainDefinition Ground { get; set; }
        public TerrainDefinition Wall { get; set; }
        public BuilderStep[] Steps { get; set; }
        public ConnectionRule[] ConnectionRules { get; set; }
        public GenericRandomPick<string>[] Population { get; set; }

        public override string ToString() => ID;
    }
}
