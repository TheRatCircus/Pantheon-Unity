// LevelEnemies.cs
// Jerome Maratina

using UnityEngine;
using Pantheon.Core;
using Pantheon.World;
using static Pantheon.Utils.RandomUtils;

namespace Pantheon.WorldGen
{
    /// <summary>
    /// Functions for distributing enemies in a level.
    /// </summary>
    public static class LevelEnemies
    {
        /// <summary>
        /// Spawn NPCs at random throughout a level.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="numNPCs">Number of NPCs to spawn.</param>
        /// <param name="pop">NPC pop set to pick from.</param>
        public static void SpawnNPCs(ref Level level, int numNPCs, RandomPickEntry<NPCType>[] pop)
        {
            if (numNPCs <= 0)
                throw new System.Exception("Number of NPCs to spawn must be non-zero.");

            for (int i = 0; i < numNPCs; i++)
            {
                GameObject prefab = Database.GetNPC(pop[RandomPick(pop)].Value).Prefab;

                Cell cell;
                int attempts = 0;
                do
                {
                    if (attempts > 100)
                        throw new System.Exception("Attempt to generate new NPC position failed.");
                    cell = level.RandomFloor();
                    attempts++;
                } while (level.Distance(cell, Game.GetPlayer().Cell) <= 7
                || cell._actor != null);
                Spawn.SpawnEnemy(prefab, level, cell);
            }
        }
    }
}
