// Interface between mouse and game map
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MouseControl : MonoBehaviour
{
    // Requisite objects
    public Tilemap tilemap;
    public Grid grid;
    public Level level;

    public Image crosshair;

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Offset tile anchor
        mousePos.x += .5f;
        mousePos.y += .5f;
        Vector3Int posInt = grid.LocalToCell(mousePos);
        Cell cell = level.GetCell((Vector2Int)posInt);
        if (cell != null)
        {
            Vector3 crosshairPos = new Vector3(cell.Position.x, cell.Position.y, 0);
            crosshair.transform.position = crosshairPos;
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (cell.Blocked)
        //    {
        //        cell.Blocked = false;
        //        cell.Opaque = false;
        //    }
        //    else
        //    {
        //        cell.Blocked = true;
        //        cell.Opaque = true;
        //    }
        //    level.RefreshFOV();
        //}

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(cell);
        }
    }
}
