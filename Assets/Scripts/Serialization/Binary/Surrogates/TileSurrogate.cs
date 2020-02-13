// TileSurrogate.cs
// Jerome Martina

using System.Runtime.Serialization;
using UnityEngine.Tilemaps;

namespace Pantheon.Serialization.Binary.Surrogates
{
    public sealed class TileSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            Tile tile = (Tile)obj;
            info.AddValue("id", tile.name);
        }

        public object SetObjectData(object obj,
            SerializationInfo info, StreamingContext context,
            ISurrogateSelector selector)
        {
            Tile tile = (Tile)obj;
            tile = Assets.GetTile<Tile>(info.GetString("id"));
            obj = tile;
            return obj;
        }
    }
}
