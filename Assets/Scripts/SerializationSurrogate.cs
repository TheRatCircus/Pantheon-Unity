// SerializationSurrogate.cs
// takatok of the Unity Forums

using UnityEngine;
using System.Runtime.Serialization;

namespace Pantheon.Utils
{
    public static class Serialization
    {
        public static SurrogateSelector GetSurrogateSelector()
        {
            SurrogateSelector selector = new SurrogateSelector();

            Vector3IntSerializationSurrogate vector3ISS =
                new Vector3IntSerializationSurrogate();
            Vector2IntSerializationSurrogate vector2ISS =
                new Vector2IntSerializationSurrogate();
            selector.AddSurrogate(typeof(Vector3Int),
                new StreamingContext(StreamingContextStates.All), vector3ISS);
            selector.AddSurrogate(typeof(Vector2Int),
                new StreamingContext(StreamingContextStates.All), vector2ISS);

            return selector;
        }
    }
}

public sealed class Vector3IntSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj,
        SerializationInfo info,
        StreamingContext context)
    {
        Vector3Int v3i = (Vector3Int)obj;
        info.AddValue("x", v3i.x);
        info.AddValue("y", v3i.y);
        info.AddValue("z", v3i.z);
    }

    public object SetObjectData(object obj,
        SerializationInfo info, StreamingContext context,
        ISurrogateSelector selector)
    {
        Vector3Int v3i = (Vector3Int)obj;
        v3i.x = (int)info.GetValue("x", typeof(int));
        v3i.y = (int)info.GetValue("y", typeof(int));
        v3i.z = (int)info.GetValue("z", typeof(int));
        obj = v3i;
        return obj;
    }
}

public sealed class Vector2IntSerializationSurrogate : ISerializationSurrogate
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
