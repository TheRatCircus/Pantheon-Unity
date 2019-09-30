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
        public static void FinishNPC(NPC npc, Level level, Cell cell)
        {
            npc.gameObject.name = npc.ActorName;
            npc.level = level;
            level.NPCs.Add(npc);
            Game.instance.AddActor(npc);
            Actor.MoveTo(npc, cell);
        }

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

            FinishNPC(npc, level, cell);
            return npc;
        }

        public static NPC SpawnBoss(ZoneBoss boss, Level level, Cell cell)
        {
            GameObject npcObj = Object.Instantiate(Database.GenericNPC,
                Helpers.V2IToV3(cell.Position),
                new Quaternion(),
                level.transform);
            NPC npc = npcObj.GetComponent<NPC>();
            npc.Initialize();

            Species species;
            if (boss.SpeciesPref != null)
                species = boss.SpeciesPref;
            else
                species = Database.GetSpecies(SpeciesRef.Human);

            npc.BuildActor(species);

            npc.MaxHealth = 50;
            npc.Speed = Actor.DefaultSpeed;
            npc.RegenRate = Actor.DefaultRegenRate;

            npc.ActorName = boss.GivenName;
            npc.SpriteRenderer.sprite = species.Sprite;
            npc.IsUnique = true;

            FinishNPC(npc, level, cell);
            return npc;
        }

        public static NPC SpawnDomainNPC(ZoneBoss boss, Level level, Cell cell)
        {
            GameObject npcObj = Object.Instantiate(Database.GenericNPC,
                Helpers.V2IToV3(cell.Position),
                new Quaternion(),
                level.transform);
            NPC npc = npcObj.GetComponent<NPC>();
            npc.Initialize();

            Species species;
            if (boss.SpeciesPref != null)
                species = boss.SpeciesPref;
            else
                species = Database.GetSpecies(SpeciesRef.Human);

            npc.BuildActor(species);

            Occupation occupation;
            if (boss.OccupationPrefs.Count > 0)
                occupation = boss.OccupationPrefs.Random(true);
            else
                occupation = Database.GetOccupation(OccupationRef.Axeman);

            npc.AssignOccupation(occupation);

            npc.MaxHealth = 20;
            npc.Speed = Actor.DefaultSpeed;
            npc.RegenRate = Actor.DefaultRegenRate;

            npc.ActorName = $"{species.DisplayName} {occupation.DisplayName}" +
                $" of {boss.GivenName}";
            npc.SpriteRenderer.sprite = species.Sprite;
            npc.AlwaysHostileToPlayer = true;

            FinishNPC(npc, level, cell);
            return npc;
        }

        /// <summary>
        /// Build a devotee based on an Idol and their Aspect.
        /// </summary>
        /// <param name="religion">The religion followed by the new devotee.</param>
        /// <param name="level">The level on which the NPC should be spawned.</param>
        /// <param name="cell">The cell in which the NPC should be spawned.</param>
        /// <returns></returns>
        public static NPC SpawnDevotee(Faction religion, Level level,
            Cell cell)
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

            string npcName = 
                $"{species.DisplayName}" +
                $" {occupation.DisplayName}" +
                $" of {idol.DisplayName}";
            npc.ActorName = npcName;
            npc.SpriteRenderer.sprite = species.Sprite;

            FinishNPC(npc, level, cell);
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

            string title;

            if (idol.Gender == Gender.Male)
                title = "Lord";
            else if (idol.Gender == Gender.Female)
                title = "Lady";
            else
                title = "Sovereign";

            string npcName = $"{idol.DisplayName}," +
                $" {title} of {idol.Aspect.DisplayName}";
            npc.ActorName = npcName;
            npc.SpriteRenderer.sprite = species.Sprite;
            npc.IsUnique = true;

            FinishNPC(npc, level, cell);
            return npc;
        }
    }
}
