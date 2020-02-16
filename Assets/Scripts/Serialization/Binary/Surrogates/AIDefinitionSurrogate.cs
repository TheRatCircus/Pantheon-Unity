// AIDefinitionSurrogate.cs
// Jerome Martina

using Pantheon.Content;
using System.Runtime.Serialization;

namespace Pantheon.Serialization.Binary.Surrogates
{
    public sealed class AIDefinitionSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info,
            StreamingContext context)
        {
            AIDefinition def = (AIDefinition)obj;
            info.AddValue("id", def.ID);
        }

        public object SetObjectData(object obj, SerializationInfo info,
            StreamingContext context, ISurrogateSelector selector)
        {
            AIDefinition def = (AIDefinition)obj;
            def = Assets.AI[info.GetString("id")];
            obj = def;
            return obj;
        }
    }
}
