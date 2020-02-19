// ExplodeCommand.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pantheon
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExplosionPattern : byte
    {
        Point,
        Line,
        Path,
        Flood,
        Square
    }
}
