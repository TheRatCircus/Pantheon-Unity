// Entity spawner
using System.Collections.Generic;
using UnityEngine;

public class MakeEntity : MonoBehaviour
{
    // Create a new projectile following a line
    public static void MakeLineProjectile(GameObject projPrefab, List<Cell> line)
    {
        GameObject magicBulletObj = Instantiate(projPrefab,
            Helpers.V2IToV3(line[0].Position),
            new Quaternion(),
            TurnController.instance.activeLevel.transform);
        Projectile magicBulletProj = magicBulletObj.GetComponent<Projectile>();
        magicBulletProj.projectileLine = line;
    }

    // Make an enemy at a cell
    public static void MakeEnemyAt(GameObject enemyPrefab, Level level, Cell cell)
    {
        GameObject enemy = Instantiate(enemyPrefab, Helpers.V2IToV3(cell.Position), new Quaternion(), level.transform);
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.level = level;
        level.actors.Add(enemyScript);
        enemyScript.MoveToCell(cell);
    }
}
