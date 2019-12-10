// SerializationSurrogate.cs
// Jerome Martina

using UnityEngine;
using System.Runtime.Serialization;

namespace Pantheon.Serialization.Binary.Surrogates
{
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
}
