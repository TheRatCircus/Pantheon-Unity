// An object which can be picked up, moved, interacted with
using UnityEngine;

public class Item : MonoBehaviour
{
    // Cell this item is in
    public Cell cell;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        TurnController.instance.OnTurnChangeEvent += OnTurnChange;
    }

    // Handle turn change event
    private void OnTurnChange()
    {
        if (cell._Actor != null)
            spriteRenderer.enabled = false;
        else
            spriteRenderer.enabled = true;
    }

    // Move this item to a given cell
    public void MoveToCell(Cell cell)
    {
        if (this.cell != null)
        {
            Cell prevCell = this.cell;
        }
        this.cell = cell;
        transform.position = Helpers.GridToVector3(cell.Position);
    }
}
