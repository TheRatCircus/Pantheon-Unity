// AudioClipSurrogate.cs
// Jerome Martina

using System.Runtime.Serialization;
using UnityEngine;

namespace Pantheon.Serialization.Binary.Surrogates
{
    public sealed class AudioClipSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info,
            StreamingContext context)
        {
            AudioClip clip = (AudioClip)obj;
            info.AddValue("id", clip.name);
        }

        public object SetObjectData(object obj, SerializationInfo info,
            StreamingContext context, ISurrogateSelector selector)
        {
            AudioClip clip = (AudioClip)obj;
            clip = Assets.Audio[info.GetString("id")];
            obj = clip;
            return obj;
        }
    }
}
