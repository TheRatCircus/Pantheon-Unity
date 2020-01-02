// Builder.cs
// Jerome Martina

namespace Pantheon.Gen
{
    /// <summary>
    /// An object used to generate a level upon request.
    /// </summary>
    [System.Serializable]
    public sealed class Builder
    {
        public string DisplayName { get; private set; }
        public string ID { get; private set; }
        public BuilderPlan Plan { get; private set; }

        public Builder(string displayName, string id, BuilderPlan plan)
        {
            DisplayName = displayName;
            ID = id;
            Plan = plan;
        }
    }
}
