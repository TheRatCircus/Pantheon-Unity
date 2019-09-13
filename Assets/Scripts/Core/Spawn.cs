// Spawn.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using Pantheon.Actors;
using Pantheon.World;
using Pantheon.Utils;

namespace Pantheon.Core
{
    /// <summary>
    /// Functions for spawning GameObject-based entities.
    /// </summary>
    public static class Spawn
    {
        /// <summary>
        /// Instantiate an NPC GameObject at a cell.
        /// </summary>
        /// <param name="npcPrefab">The prefab of the NPC to spawn.</param>
        /// <param name="level">The level on which the NPC should be spawned.</param>
        /// <param name="cell"></param>
        /// <returns>The NPC script component of the new NPC.</returns>
        public static NPC SpawnNPC(GameObject npcPrefab, Level level, Cell cell)
        {
            GameObject npcObj = Object.Instantiate(
                npcPrefab,
                Helpers.V2IToV3(cell.Position),
                new Quaternion(),
                level.transform
                );
            NPC npc = npcObj.GetComponent<NPC>();
            npc.level = level;
            level.NPCs.Add(npc);
            Game.instance.AddActor(npc);
            Actor.MoveTo(npc, cell);
            return npc;
        }
    }

}
