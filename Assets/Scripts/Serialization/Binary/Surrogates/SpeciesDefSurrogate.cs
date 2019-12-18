// SpeciesDefSurrogate.cs
// Jerome Martina

using Pantheon.Content;
using Pantheon.Core;
using System.Runtime.Serialization;

namespace Pantheon.Serialization.Binary.Surrogates
{
    public sealed class SpeciesDefSurrogate : ISerializationSurrogate
    {
        private AssetLoader loader;

        public SpeciesDefSurrogate(AssetLoader loader)
        {
            this.loader = loader;
        }

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
            species = loader.LoadSpeciesDef(info.GetString("id"));
            obj = species;
            return obj;
        }
    }
}
