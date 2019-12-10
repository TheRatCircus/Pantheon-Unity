// EntityTemplateSurrogate.cs
// Jerome Martina

using System.Runtime.Serialization;
using Pantheon.Core;

namespace Pantheon.Serialization.Binary.Surrogates
{
    public sealed class EntityTemplateSurrogate : ISerializationSurrogate
    {
        private AssetLoader Loader;

        public EntityTemplateSurrogate(AssetLoader loader)
        {
            Loader = loader;
        }

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
            et = Loader.LoadTemplate(info.GetString("id"));
            obj = et;
            return obj;
        }
    }
}
