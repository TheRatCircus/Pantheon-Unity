// NPCSpawn.cs
// Jerome Maratina

using System;
using UnityEngine;
using Pantheon.Core;
using Pantheon.World;
using static Pantheon.Utils.RandomUtils;
using Pantheon.Utils;

namespace Pantheon.WorldGen
{
    /// <summary>
    /// Functions for distributing enemies in a level.
    /// </summary>
    public sealed class NPCSpawn
    {
        public Level Level { get; private set; }
        public int MinSpawns { get; private set; }
        public int MaxSpawns { get; private set; }
        public RandomPickEntry<NPCType>[] Pop { get; private set; }

        private NPCWrapper currentNPC = null;

        public NPCSpawn(Level level, int minSpawns, int maxSpawns,
            RandomPickEntry<NPCType>[] pop)
        {
            Level = level;
            MinSpawns = minSpawns;
            MaxSpawns = maxSpawns;
            Pop = pop;
        }

        /// <summary>
        /// Spawn NPCs at random throughout a level.
        /// </summary>
        /// <param name="level">Level to modify by reference.</param>
        /// <param name="numNPCs">Number of NPCs to spawn.</param>
        /// <param name="pop">NPC pop set to pick from.</param>
        public void SpawnNPCs()
        {
            if (MaxSpawns <= 0)
                throw new Exception("Number of NPCs to spawn must be non-zero.");

            int numSpawns = Game.PRNG.Next(MinSpawns, MaxSpawns);

            for (int i = 0; i < numSpawns; i++)
            {
                currentNPC = Database.GetNPC(Pop.RandomPick(true));

                if (currentNPC.PackSpawn)
                {
                    Cell cell;
                    int attempts = 0;
                    do
                    {
                        if (attempts > 100)
                            throw new Exception
                                ($"No valid NPC spawn position found after " +
                                $"{attempts} tries.");

                        cell = Level.RandomFloor();
                        attempts++;

                    } while (Level.Distance(cell, Game.GetPlayer().Cell) <= 21
                    || cell.Actor != null);

                    int numPackSpawns = Game.PRNG.Next(currentNPC.MinPackSize,
                        currentNPC.MaxPackSize);

                    Algorithms.FloodFill(Level, cell, PackSpawnNPC,
                        numPackSpawns);

                    // Bump counter to reflect number of spawns in pack
                    i--;
                    i += numPackSpawns;
                }
                else
                {
                    Cell cell;
                    int attempts = 0;
                    do
                    {
                        if (attempts > 100)
                            throw new Exception
                                ($"No valid NPC spawn position found after " +
                                $"{attempts} tries.");

                        cell = Level.RandomFloor();
                        attempts++;

                    } while (Level.Distance(cell, Game.GetPlayer().Cell) <= 15
                    || cell.Actor != null);

                    Spawn.SpawnNPC(currentNPC.Prefab, Level, cell);
                }
            }
        }

        /// <summary>
        /// Roll to spawn an NPC as part of a pack.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private bool PackSpawnNPC(Cell cell)
        {
            if (OneChanceIn(3))
            {
                Spawn.SpawnNPC(currentNPC.Prefab, Level, cell);
                return true;
            }
            else
                return false;
        }
    }
}
