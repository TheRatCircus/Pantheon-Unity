// NPCPops.cs
// Jerome Martina

using static Pantheon.Utils.RandomUtils;

/// <summary>
/// Defines the NPC populations of any given zone.
/// </summary>
public static class NPCPops
{
    public static RandomPickEntry<NPCType>[] ValleyCentre =
    {
        new RandomPickEntry<NPCType>(512, NPCType.Goblin)
    };

    public static RandomPickEntry<NPCType>[] ValleyNorth =
    {
        new RandomPickEntry<NPCType>(512, NPCType.Goblin),
        new RandomPickEntry<NPCType>(64, NPCType.DreadHamster)
    };
}
