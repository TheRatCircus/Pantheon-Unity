// Entity spawner
using UnityEngine;

public class MakeEntity : MonoBehaviour
{
    // Singleton
    public static MakeEntity instance;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        instance = this;
    }

    // Make an enemy at a cell
    public void MakeEnemyAt(GameObject enemyPrefab, Level level, Cell cell)
    {
        GameObject enemy = Instantiate(enemyPrefab, Helpers.V2IToV3(cell.Position), new Quaternion(), level.transform);
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.level = level;
        level.actors.Add(enemyScript);
        enemyScript.MoveToCell(cell);
    }

    // Make a corpse at a cell
    public void MakeCorpseAt(GameObject corpsePrefab, Level level, Cell cell)
    {
        GameObject corpse = Instantiate(corpsePrefab, Helpers.V2IToV3(cell.Position), new Quaternion(), level.transform);
        Item corpseItem = corpse.GetComponent<Item>();
        corpseItem.MoveToCell(cell);
    }
}
