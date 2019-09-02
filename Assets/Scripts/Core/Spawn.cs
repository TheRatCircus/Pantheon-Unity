// Entity spawner
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    // Create a new projectile following a line
    public static void MakeLineProjectile(GameObject projPrefab, List<Cell> line)
    {
        GameObject magicBulletObj = Instantiate(projPrefab,
            Helpers.V2IToV3(line[0].Position),
            new Quaternion(),
            Game.instance.activeLevel.transform);
        Projectile magicBulletProj = magicBulletObj.GetComponent<Projectile>();
        magicBulletProj.ProjectileLine = line;
        magicBulletProj.FireProjectile();
    }

    // Make an enemy at a cell
    public static Enemy MakeEnemyAt(GameObject enemyPrefab, Level level, Cell cell)
    {
        GameObject enemyObj = Instantiate(enemyPrefab, Helpers.V2IToV3(cell.Position), new Quaternion(), level.transform);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.level = level;
        level.enemies.Add(enemy);
        Game.instance.AddActor(enemy);
        Actor.MoveTo(enemy, cell);
        return enemy;
    }
}
