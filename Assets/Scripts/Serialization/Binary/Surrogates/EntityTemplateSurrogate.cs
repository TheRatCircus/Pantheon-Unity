// EntityTemplateSurrogate.cs
// Jerome Martina

using System.Runtime.Serialization;
using Pantheon.Core;
using Pantheon.ECS;

namespace Pantheon.Serialization.Binary.Surrogates
{
    public sealed class EntityTemplateSurrogate : ISerializationSurrogate
    {
        private AssetLoader loader;

        public EntityTemplateSurrogate(AssetLoader loader)
        {
            this.loader = loader;
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
            et = loader.LoadTemplate(info.GetString("id"));
            obj = et;
            return obj;
        }
    }
}
