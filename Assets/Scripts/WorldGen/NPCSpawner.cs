// NPCSpawner.cs
// Jerome Maratina

using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System;
using static Pantheon.Utils.RandomUtils;

namespace Pantheon.WorldGen
{
    /// <summary>
    /// Functions for distributing enemies in a level.
    /// </summary>
    public abstract class NPCSpawner
    {
        public Level Level { get; private set; }
        protected int MinSpawns { get; private set; }
        protected int MaxSpawns { get; private set; }

        

        public NPCSpawner(Level level, int minSpawns, int maxSpawns)
        {
            Level = level;
            MinSpawns = minSpawns;
            MaxSpawns = maxSpawns;
        }

        public abstract void Run();

        /// <summary>
        /// Roll to spawn an NPC as part of a pack.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        protected abstract bool PackSpawnNPC(Cell cell);
    }

    /// <summary>
    /// Use to spawn ambient zone NPCs.
    /// </summary>
    public sealed class AmbientSpawner : NPCSpawner
    {
        public GenericRandomPick<NPCID>[] Pop { get; private set; }
        private NPCWrapper currentNPC = null;

        public AmbientSpawner(Level level, int minSpawns, int maxSpawns,
            GenericRandomPick<NPCID>[] pop) : base(level, minSpawns, maxSpawns)
        {
            Pop = pop;
        }

        public override void Run()
        {
            if (MaxSpawns <= 0)
                throw new Exception
                    ("Number of NPCs to spawn must be non-zero.");

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
                        if (attempts >= 100)
                            throw new Exception
                                ($"No valid NPC spawn position found after " +
                                $"{attempts} tries.");

                        cell = Level.RandomFloor();
                        attempts++;

                    } while (Level.Distance(cell, Game.GetPlayer().Cell) <= 15
                    || cell.Actor != null);

                    int numPackSpawns = Game.PRNG.Next(currentNPC.MinPackSize,
                        currentNPC.MaxPackSize);

                    Algorithms.FloodFill(Level, cell, PackSpawnNPC,
                        numPackSpawns);

                    // Bump counter to reflect number of spawns in pack
                    i += numPackSpawns - 1;
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

        protected override bool PackSpawnNPC(Cell cell)
        {
            if (cell.Actor == null && OneChanceIn(3, true))
            {
                Spawn.SpawnNPC(currentNPC.Prefab, Level, cell);
                return true;
            }
            else
                return false;
        }
    }

    /// <summary>
    /// Use to spawn NPCs belonging to a zone boss' domain.
    /// </summary>
    public sealed class DomainSpawner : NPCSpawner
    {
        private Zone Zone { get; set; }

        public DomainSpawner(Level level, Zone zone, int minSpawns,
            int maxSpawns) : base(level, minSpawns, maxSpawns)
        {
            Zone = zone;
        }

        public override void Run()
        {
            int numSpawns = Game.PRNG.Next(MinSpawns, MaxSpawns);

            for (int i = 0; i < numSpawns; i++)
            {
                Cell cell;
                int attempts = 0;
                do
                {
                    if (attempts >= 100)
                        throw new Exception
                            ($"No valid NPC spawn position found after " +
                            $"{attempts} tries.");

                    cell = Level.RandomFloor();
                    attempts++;

                } while (cell.Actor != null);

                int numPackSpawns = Game.PRNG.Next(2, 5);

                Algorithms.FloodFill(Level, cell, PackSpawnNPC,
                    numPackSpawns);

                // Bump counter to reflect number of spawns in pack
                i += numPackSpawns - 1;
            }
        }

        protected override bool PackSpawnNPC(Cell cell)
        {
            if (cell.Actor == null && OneChanceIn(3, true))
            {
                Spawn.SpawnDomainNPC(Zone.Boss, Level, cell);
                return true;
            }
            else
                return false;
        }
    }
}
