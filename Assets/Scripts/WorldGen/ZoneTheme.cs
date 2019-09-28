// ZoneTheme.cs
// Jerome Martina

using System;
using Pantheon.World;

namespace Pantheon.WorldGen
{
    public sealed class ZoneTheme
    {
        public string DisplayName { get; set; }
        public ThemeRef ThemeRef { get; set; }
        public Action<Level> OuterGenDelegate { get; set; }
        public Action<Level> CentreGenDelegate { get; set; }

        public ZoneTheme(string displayName, ThemeRef themeRef,
            Action<Level> outerGenDelegate, Action<Level> centreGenDelegate)
        {
            DisplayName = displayName;
            ThemeRef = themeRef;
            OuterGenDelegate = outerGenDelegate;
            CentreGenDelegate = centreGenDelegate;
        }
    }

    public enum ThemeRef
    {
        None,
        Valley
    }

    public static class ThemeDefs
    {
        public static ZoneTheme _valley = new ZoneTheme(
            "Valley",
            ThemeRef.Valley,
            Zones.GenerateOuterValley,
            Zones.GenerateCentralValley);
    }
}