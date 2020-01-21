// PrefabSurrogate.cs
// Jerome Martina

using System.Runtime.Serialization;
using UnityEngine;

namespace Pantheon.Serialization.Binary.Surrogates
{
    public sealed class PrefabSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info,
            StreamingContext context)
        {
            GameObject go = (GameObject)obj;
            info.AddValue("id", go.name);
        }

        public object SetObjectData(object obj, SerializationInfo info,
            StreamingContext context, ISurrogateSelector selector)
        {
            GameObject go = (GameObject)obj;
            go = Assets.Prefabs[info.GetString("id")];
            obj = go;
            return obj;
        }
    }
}
