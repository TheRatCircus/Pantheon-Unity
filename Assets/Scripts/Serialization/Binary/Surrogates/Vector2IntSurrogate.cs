// SerializationSurrogate.cs
// Jerome Martina

using UnityEngine;
using System.Runtime.Serialization;

namespace Pantheon.Serialization.Binary.Surrogates
{
    public sealed class Vector2IntSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            Vector2Int v2i = (Vector2Int)obj;
            info.AddValue("x", v2i.x);
            info.AddValue("y", v2i.y);
        }

        public object SetObjectData(object obj,
            SerializationInfo info, StreamingContext context,
            ISurrogateSelector selector)
        {
            Vector2Int v2i = (Vector2Int)obj;
            v2i.x = (int)info.GetValue("x", typeof(int));
            v2i.y = (int)info.GetValue("y", typeof(int));
            obj = v2i;
            return obj;
        }
    }
}
