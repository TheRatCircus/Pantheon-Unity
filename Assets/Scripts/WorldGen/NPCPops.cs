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
        public static GenericRandomPick<NPCID>[] _startingValley =
        {
            new GenericRandomPick<NPCID>(256, NPCID.RagingGoose),
            new GenericRandomPick<NPCID>(256, NPCID.Coyote)
        };
    }
}
