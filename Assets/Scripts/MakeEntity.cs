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

    // Make a corpse at a cell
    public void MakeCorpseAt(GameObject corpsePrefab, Level level, Cell cell)
    {
        GameObject corpse = Instantiate(corpsePrefab, Helpers.V2IToV3(cell.Position), new Quaternion(), level.transform);
        Item corpseItem = corpse.GetComponent<Item>();
        corpseItem.MoveToCell(cell);
    }
}
