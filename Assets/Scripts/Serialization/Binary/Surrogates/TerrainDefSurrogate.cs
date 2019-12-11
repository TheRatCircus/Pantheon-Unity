// TerrainDefSurrogate.cs
// Jerome Martina

using System.Runtime.Serialization;
using Pantheon.Core;

namespace Pantheon.Serialization.Binary.Surrogates
{
    /// <summary>
    /// Serializes a TerrainDef as its ID and reloads it upon loading a game.
    /// </summary>
    public sealed class TerrainDefSurrogate : ISerializationSurrogate
    {
        private AssetLoader loader;

        public TerrainDefSurrogate(AssetLoader loader)
        {
            this.loader = loader;
        }

        public void GetObjectData(object obj, SerializationInfo info,
            StreamingContext context)
        {
            TerrainDefinition td = (TerrainDefinition)obj;
            info.AddValue("id", td.name);
        }

        public object SetObjectData(object obj, SerializationInfo info,
            StreamingContext context, ISurrogateSelector selector)
        {
            TerrainDefinition td = (TerrainDefinition)obj;
            td = loader.Load<TerrainDefinition>(info.GetString("id"));
            obj = td;
            return obj;
        }
    }
}
