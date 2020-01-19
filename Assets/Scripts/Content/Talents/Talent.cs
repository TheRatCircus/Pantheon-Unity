// Talent.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pantheon.Commands;
using UnityEngine;

namespace Pantheon.Content.Talents
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TargetingScheme
    {
        Adjacent
    }

    public abstract class Talent
    {
        [JsonIgnore] public abstract TargetingScheme TargetingScheme { get; }
        [JsonIgnore] public abstract Sprite Icon { get; }

        public abstract CommandResult Cast(Entity caster);
    }
}
