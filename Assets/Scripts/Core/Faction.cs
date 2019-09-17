// Faction.cs
// Jerome Martina

using Pantheon.Actors;

namespace Pantheon.Core
{
    public class Faction
    {
        public string DisplayName { get; set; }
        public string RefName { get; set; }
        public FactionType Type { get; set; }
        public Idol Idol { get; set; }

        // Traits granted to members
        // Abilities available to members

        public Faction(string displayName, string refName, FactionType type, Idol idol)
        {
            DisplayName = displayName;
            RefName = refName;
            Type = type;
            Idol = idol;
        }
    }

    public enum FactionType
    {
        None,
        Nature,
        Religion
    }
}
