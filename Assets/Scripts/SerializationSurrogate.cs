// SerializationSurrogate.cs
// Jerome Martina

using UnityEngine;
using System.Runtime.Serialization;
using Pantheon;

namespace Pantheon.Utils
{
    public static class Serialization
    {
        public static SurrogateSelector GetSurrogateSelector()
        {
            SurrogateSelector selector = new SurrogateSelector();

            Vector3IntSurrogate vector3ISS = new Vector3IntSurrogate();
            Vector2IntSurrogate vector2ISS = new Vector2IntSurrogate();
            selector.AddSurrogate(typeof(Vector3Int),
                new StreamingContext(StreamingContextStates.All), vector3ISS);
            selector.AddSurrogate(typeof(Vector2Int),
                new StreamingContext(StreamingContextStates.All), vector2ISS);

            return selector;
        }
    }
}

public sealed class ScriptableObjectSurrogate : ISerializationSurrogate
{
    private AssetLoader Loader;

    public ScriptableObjectSurrogate(AssetLoader loader)
    {
        Loader = loader;
    }

    public void GetObjectData(object obj, SerializationInfo info,
        StreamingContext context)
    {
        ScriptableObject so = (ScriptableObject)obj;
        info.AddValue("id", so.name);
    }

    public object SetObjectData(object obj, SerializationInfo info,
        StreamingContext context, ISurrogateSelector selector)
    {
        ScriptableObject so = (ScriptableObject)obj;
        so = Loader.Load<ScriptableObject>((string)info.GetValue("id", typeof(string)));
        obj = so;
        return obj;
    }
}

public sealed class SpriteSurrogate : ISerializationSurrogate
{
    private AssetLoader Loader;

    public SpriteSurrogate(AssetLoader loader)
    {
        Loader = loader;
    }

    public void GetObjectData(object obj, SerializationInfo info,
        StreamingContext context)
    {
        Sprite sprite = (Sprite)obj;
        info.AddValue("id", sprite.name);
    }

    public object SetObjectData(object obj, SerializationInfo info,
        StreamingContext context, ISurrogateSelector selector)
    {
        Sprite sprite = (Sprite)obj;
        sprite = Loader.Load<Sprite>(info.GetString("id"));
        obj = sprite;
        return obj;
    }
}

public sealed class TerrainDefSurrogate : ISerializationSurrogate
{
    private AssetLoader Loader;

    public TerrainDefSurrogate(AssetLoader loader)
    {
        Loader = loader;
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
        td = Loader.Load<TerrainDefinition>(info.GetString("id"));
        obj = td;
        return obj;
    }
}

public sealed class Vector3IntSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info,
        StreamingContext context)
    {
        Vector3Int v3i = (Vector3Int)obj;
        info.AddValue("x", v3i.x);
        info.AddValue("y", v3i.y);
        info.AddValue("z", v3i.z);
    }

    public object SetObjectData(object obj, SerializationInfo info,
        StreamingContext context, ISurrogateSelector selector)
    {
        Vector3Int v3i = (Vector3Int)obj;
        v3i.x = (int)info.GetValue("x", typeof(int));
        v3i.y = (int)info.GetValue("y", typeof(int));
        v3i.z = (int)info.GetValue("z", typeof(int));
        obj = v3i;
        return obj;
    }
}

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
