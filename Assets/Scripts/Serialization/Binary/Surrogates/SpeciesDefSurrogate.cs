// SpeciesDefSurrogate.cs
// Jerome Martina

using Pantheon.Content;
using System.Runtime.Serialization;

namespace Pantheon.Serialization.Binary.Surrogates
{
    public sealed class SpeciesDefSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info,
            StreamingContext context)
        {
            SpeciesDefinition species = (SpeciesDefinition)obj;
            info.AddValue("id", species.ID);
        }

        public object SetObjectData(object obj, SerializationInfo info,
            StreamingContext context, ISurrogateSelector selector)
        {
            SpeciesDefinition species = (SpeciesDefinition)obj;
            species = Assets.Species[info.GetString("id")];
            obj = species;
            return obj;
        }
    }
}
