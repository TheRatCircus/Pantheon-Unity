// Species.cs
// Jerome Martina

using Pantheon.ECS.Templates;

namespace Pantheon.ECS.Components
{
    [System.Serializable]
    //[Newtonsoft.Json.JsonConverter(typeof(ComponentConverter))]
    public sealed class Species : BaseComponent
    {
        public string ID { get; private set; } = "DEFAULT_SPECIES_ID";
        public string Name { get; private set; } = "DEFAULT_SPECIES_NAME";
        public EntityTemplate[] Appendages { get; private set; }

        public Species(string id, string name, EntityTemplate[] appendages)
        {
            ID = id;
            Name = name;
            Appendages = appendages;
        }

        public override BaseComponent Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
