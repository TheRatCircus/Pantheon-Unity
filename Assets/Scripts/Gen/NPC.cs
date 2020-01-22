// NPC.cs
// Jerome Martina

using Pantheon.Content;
using Pantheon.Core;
using Pantheon.Utils;
using Pantheon.World;
using System;
using Random = UnityEngine.Random;

namespace Pantheon.Gen
{
    public static class NPC
    {
        public static void PopulateNPCs(BuilderPlan plan, Level level)
        {
            int minSpawns = level.CellCount / 100;
            int maxSpawns = level.CellCount / 90;
            int numSpawns = Random.Range(minSpawns, maxSpawns);

            for (int i = 0; i < numSpawns; i++)
            {
                string id = GenericRandomPick<string>.Pick(plan.Population);
                EntityTemplate template = Assets.Templates[id];

                Cell cell;
                int attempts = 0;
                do
                {
                    if (attempts > 100)
                        throw new Exception
                            ($"No valid NPC spawn position found after " +
                            $"{attempts} tries.");

                    cell = level.RandomCell(true);
                    attempts++;

                } while (!Cell.Walkable(cell));

                Spawn.SpawnActor(template, level, cell);
            }
        }
    }
}