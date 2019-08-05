// Entity spawner
using UnityEngine;

public class MakeEntity : MonoBehaviour
{
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
