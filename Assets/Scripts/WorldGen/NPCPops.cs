// NPCPops.cs
// Jerome Martina

using static Pantheon.Utils.RandomUtils;

namespace Pantheon.WorldGen
{
    /// <summary>
    /// Defines ambient NPC populations for zone themes.
    /// </summary>
    public static class NPCPops
    {
        public static RandomPickEntry<NPCType>[] _startingValley =
        {
            new RandomPickEntry<NPCType>(256, NPCType.RagingGoose),
            new RandomPickEntry<NPCType>(256, NPCType.Coyote)
        };
    }
}
