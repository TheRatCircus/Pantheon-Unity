// EntityTemplateSurrogate.cs
// Jerome Martina

using System.Runtime.Serialization;
using Pantheon.Content;

namespace Pantheon.Serialization.Binary.Surrogates
{
    public sealed class EntityTemplateSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info,
            StreamingContext context)
        {
            EntityTemplate et = (EntityTemplate)obj;
            info.AddValue("id", et.ID);
        }

        public object SetObjectData(object obj, SerializationInfo info,
            StreamingContext context, ISurrogateSelector selector)
        {
            EntityTemplate et = (EntityTemplate)obj;
            et = Assets.Templates[info.GetString("id")];
            obj = et;
            return obj;
        }
    }
}
