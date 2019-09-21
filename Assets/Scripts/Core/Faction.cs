// Faction.cs
// Jerome Martina

using System.Collections.Generic;
using Pantheon.Actors;

namespace Pantheon.Core
{
    public class Faction
    {
        public string DisplayName { get; set; }
        public string RefName { get; set; }
        public FactionType Type { get; set; }
        public Idol Idol { get; set; }
        public bool HostileToPlayer { get; set; }

        public List<TraitRef> GrantedTraits { get; set; }
        // Abilities available to members

        public Faction(string displayName, string refName, FactionType type,
            Idol idol)
        {
            DisplayName = displayName;
            RefName = refName;
            Type = type;
            Idol = idol;
        }

        public override string ToString() => DisplayName;
    }

    public enum FactionType
    {
        None,
        Nature,
        Religion
    }
}
