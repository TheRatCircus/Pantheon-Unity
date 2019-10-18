// Relic.cs
// Jerome Martina

using Pantheon.Core;
using static Pantheon.Utils.RandomUtils;
using System.Collections.Generic;
using System.Linq;

namespace Pantheon.WorldGen
{
    public static class Relic
    {
        public static void NameRelic(Item relic)
        {
            string prefix = GetRelicAffix(true);
            string suffix = GetRelicAffix(false);

            relic.DisplayName = $"{relic.BaseName}: {prefix} {suffix}";
        }

        public static string GetRelicAffix(bool adjective)
        {
            List<string> groups = Database.RelicNames.text.Split(';').
                ToList();
            // First get a group
            string group = groups[Game.PRNG.Next(groups.Count)];
            string[] words = group.Split(',');
            return words[adjective ? 1 : 0];
        }
    }
}
