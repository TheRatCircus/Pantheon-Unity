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
        public static GenericRandomPick<NPCType>[] _startingValley =
        {
            new GenericRandomPick<NPCType>(256, NPCType.RagingGoose),
            new GenericRandomPick<NPCType>(256, NPCType.Coyote)
        };
    }
}
