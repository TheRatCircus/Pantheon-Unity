// DamageType.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pantheon
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DamageType
    {
        None,
        Slashing,
        Piercing,
        Bludgeoning,
        Searing,
        Freezing,
        Entropic
    }
}