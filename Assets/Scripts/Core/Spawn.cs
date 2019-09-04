// Spawn.cs
// Jerome Martina

using Pantheon.Utils;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Functions for spawning 
/// </summary>
public static class Spawn
{
    // Create a new projectile following a line
    public static void MakeLineProjectile(GameObject projPrefab, List<Cell> line)
    {
        GameObject magicBulletObj = Object.Instantiate(
            projPrefab,
            Helpers.V2IToV3(line[0].Position),
            new Quaternion(),
            Game.instance.activeLevel.transform
            );
        Projectile magicBulletProj = magicBulletObj.GetComponent<Projectile>();
        magicBulletProj.ProjectileLine = line;
        magicBulletProj.FireProjectile();
    }

    /// <summary>
    /// Instantiate an enemy GameObject at a cell.
    /// </summary>
    /// <param name="enemyPrefab">The prefab of the enemy to spawn.</param>
    /// <param name="level">The level on which the enemy should be spawned.</param>
    /// <param name="cell"></param>
    /// <returns>The Enemy script component of the new enemy.</returns>
    public static Enemy SpawnEnemy(GameObject enemyPrefab, Level level, Cell cell)
    {
        GameObject enemyObj = Object.Instantiate(
            enemyPrefab,
            Helpers.V2IToV3(cell.Position),
            new Quaternion(),
            level.transform
            );
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.level = level;
        level.Enemies.Add(enemy);
        Game.instance.AddActor(enemy);
        Actor.MoveTo(enemy, cell);
        return enemy;
    }
}
