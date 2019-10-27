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
        public static GenericRandomPick<string>[] _startingValley =
        {
            new GenericRandomPick<string>(256, ID.NPC._ragingGoose),
            new GenericRandomPick<string>(256, ID.NPC._coyote)
        };
    }
}
