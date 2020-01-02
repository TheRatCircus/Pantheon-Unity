// PrefabSurrogate.cs
// Jerome Martina

using System.Runtime.Serialization;
using Pantheon.Core;
using UnityEngine;

namespace Pantheon.Serialization.Binary.Surrogates
{
    public sealed class PrefabSurrogate : ISerializationSurrogate
    {
        private AssetLoader loader;

        public PrefabSurrogate(AssetLoader loader)
        {
            this.loader = loader;
        }

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
            go = loader.Load<GameObject>("id");
            obj = go;
            return obj;
        }
    }
}
