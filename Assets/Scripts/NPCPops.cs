// NPCPops.cs
// Jerome Martina

using static Pantheon.Utils.RandomUtils;

/// <summary>
/// Defines the NPC populations of any given zone.
/// </summary>
public static class NPCPops
{
    /// <summary>
    /// Population set for the Valley zone.
    /// </summary>
    public static RandomPickEntry<NPCType>[] ValleyPop =
    {
        new RandomPickEntry<NPCType>(512, NPCType.Goblin)
    };
}
