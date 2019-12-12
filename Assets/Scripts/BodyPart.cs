// BodyPart.cs
// Jerome Martina

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pantheon.Components;
using UnityEngine;

namespace Pantheon
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BodyPartType
    {
        Head,
        Arms,
        Legs,
        Teeth,
        Wings
    }

    [System.Serializable]
    public sealed class BodyPart
    {
        [SerializeField] private string id = "DEFAULT_BODYPART_ID";
        [SerializeField] private string name = "DEFAULT_BODYPART_NAME";
        [SerializeField] private BodyPartType type;
        [SerializeField] private int moveSpeedModifier;
        [SerializeField] private Melee melee;

        public string ID => id;
        public string Name => name;
        public BodyPartType Type => type;
        public int MoveSpeedModifier => moveSpeedModifier;
        public Melee Melee => melee;

        public BodyPart() { }

        public BodyPart(string id)
        {
            this.id = id;
        }

        public BodyPart(string id, string name, BodyPartType type,
            int moveSpeedModifier, Melee melee)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.moveSpeedModifier = moveSpeedModifier;
            this.melee = melee;
        }
    }
}
