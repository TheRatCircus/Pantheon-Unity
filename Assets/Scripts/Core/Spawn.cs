// Spawn.cs
// Jerome Martina

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
        /// Instantiate an NPC GameObject at a cell based on a prefab.
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
                level.transform);
            NPC npc = npcObj.GetComponent<NPC>();
            npc.gameObject.name = npc.ActorName;
            npc.level = level;
            level.NPCs.Add(npc);
            Game.instance.AddActor(npc);
            Actor.MoveTo(npc, cell);
            return npc;
        }

        /// <summary>
        /// Build a devotee based on an Idol and their Aspect.
        /// </summary>
        /// <param name="religion">The religion followed by the new devotee.</param>
        /// <param name="level">The level on which the NPC should be spawned.</param>
        /// <param name="cell">The cell in which the NPC should be spawned.</param>
        /// <returns></returns>
        public static NPC SpawnDevotee(Faction religion, Level level, Cell cell)
        {
            Idol idol = religion.Idol;

            GameObject npcObj = Object.Instantiate(Database.GenericNPC,
                Helpers.V2IToV3(cell.Position),
                new Quaternion(),
                level.transform);
            NPC npc = npcObj.GetComponent<NPC>();
            npc.Initialize();
            npc.Faction = religion;

            Occupation occupation = idol.Aspect.Occupations.Random(true);
            Species species = idol.Aspect.Species.Random(true);
            npc.BuildActor(species);
            // AssignOccupation won't work without limbs, so defer it
            npc.AssignOccupation(occupation);

            npc.MaxHealth = 20;
            npc.Speed = Actor.DefaultSpeed;
            npc.RegenRate = Actor.DefaultRegenRate;
            npc.MoveSpeed = Actor.DefaultMoveSpeed;

            string npcName = $"{species.DisplayName} {occupation.DisplayName} of {idol.DisplayName}";
            npc.ActorName = npcName;
            npc.gameObject.name = npc.ActorName;
            npc.SpriteRenderer.sprite = species.Sprite;

            npc.level = level;
            level.NPCs.Add(npc);
            Game.instance.AddActor(npc);
            Actor.MoveTo(npc, cell);
            return npc;
        }

        public static NPC SpawnIdol(Idol idol, Level level, Cell cell)
        {
            GameObject npcObj = Object.Instantiate(Database.GenericNPC,
                Helpers.V2IToV3(cell.Position),
                new Quaternion(),
                level.transform);
            NPC npc = npcObj.GetComponent<NPC>();
            npc.Initialize();
            npc.Faction = idol.Religion;

            Occupation occupation = idol.Aspect.Occupations.Random(true);
            Species species = idol.Aspect.Species.Random(true);
            npc.BuildActor(species);
            // AssignOccupation won't work without limbs, so defer it
            npc.AssignOccupation(occupation);

            npc.MaxHealth = 100;
            npc.Speed = Actor.DefaultSpeed;
            npc.RegenRate = Actor.DefaultRegenRate;
            npc.MoveSpeed = Actor.DefaultMoveSpeed;

            string title;

            if (idol.Gender == Gender.Male)
                title = "Lord";
            else if (idol.Gender == Gender.Female)
                title = "Lady";
            else
                title = "Sovereign";

            string npcName = $"{idol.DisplayName}, {title} of {idol.Aspect.DisplayName}";
            npc.ActorName = npcName;
            npc.SpriteRenderer.sprite = species.Sprite;
            npc.IsUnique = true;

            npc.level = level;
            level.NPCs.Add(npc);
            Game.instance.AddActor(npc);
            Actor.MoveTo(npc, cell);
            return npc;
        }
    }
}
