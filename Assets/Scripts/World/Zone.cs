// Zone.cs
// Jerome Martina

using Pantheon.Actors;
using Pantheon.Utils;
using Pantheon.WorldGen;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon.World
{
    /// <summary>
    /// A grouping of 9 levels, usually ruled by a boss.
    /// </summary>
    public sealed class Zone
    {
        public string FullName => $"{Theme.DisplayName} of {ZoneName}";
        public string ZoneName { get; private set; }
        public string RefName { get; private set; }
        public Vector2Int Position { get; private set; }

        public ZoneTheme Theme { get; private set; }
        public Level[,] Levels { get; private set; } = new Level[3, 3];
        public ZoneBoss Boss { get; set; }

        public Zone(ZoneTheme theme, Vector2Int position)
        {
            Theme = theme;
            // TODO: Random name generation
            ZoneName = "Dorn";
            Position = position;
        }
    }

    public sealed class ZoneBoss
    {
        public string FullName { get; private set; } = "NO_DISPLAY_NAME";
        public string GivenName { get; private set; } = "NO_GIVEN_NAME";
        public string RefName { get; private set; } = "NO_REF";
        public Gender Gender { get; private set; } = Gender.None;

        // Boss may be (and thus prefer) a certain species
        public Species SpeciesPref { get; private set; } = null;
        public List<Occupation> OccupationPrefs { get; private set; }
            = new List<Occupation>();

        public ZoneBoss(Zone zone)
        {
            Gender = RandomUtils.CoinFlip(true) ? Gender.Male : Gender.Female;
            string title =
                Gender == Gender.Male ? "Petty King" : "Petty Queen";

            // TODO: Random name generation
            GivenName = "Chandalar";
            RefName = GivenName.ToLower();
            FullName = $"{GivenName}, Petty {title} of {zone.ZoneName}";

            //if (RandomUtils.OneChanceIn(10, true))
            //    SpeciesPref = Core.Database.GetSpecies
            //        (RandomUtils.EnumRandom<SpeciesRef>(true));
        }

        public override string ToString() => $"{RefName}";
    }
}
