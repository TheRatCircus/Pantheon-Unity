// TalentSurrogate.cs
// Jerome Martina

using System.Runtime.Serialization;

namespace Pantheon.Serialization.Binary.Surrogates
{
    public sealed class TalentSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info,
            StreamingContext context)
        {
            Talent talent = (Talent)obj;
            info.AddValue("id", talent.ID);
        }

        public object SetObjectData(object obj, SerializationInfo info,
            StreamingContext context, ISurrogateSelector selector)
        {
            Talent talent = (Talent)obj;
            talent = Assets.Talents[info.GetString("id")];
            obj = talent;
            return obj;
        }
    }
}
