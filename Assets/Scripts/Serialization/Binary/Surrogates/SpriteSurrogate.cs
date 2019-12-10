// SpriteSurrogate.cs
// Jerome Martina

using UnityEngine;
using System.Runtime.Serialization;
using Pantheon.Core;

namespace Pantheon.Serialization.Binary.Surrogates
{
    /// <summary>
    /// Serializes a sprite as its asset ID and reloads it upon loading a save.
    /// </summary>
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
}
