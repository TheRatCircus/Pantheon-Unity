// Interface between mouse and game map
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;

public class MouseControl : MonoBehaviour
{
    // Requisite objects
    public Tilemap tilemap;
    public Grid grid;
    public Level level;
    public Player player;

    // UI elements
    public Image crosshair;

    // Update is called once per frame
    void Update()
    {
        CellActions();
    }

    private void CellActions()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Offset tile anchor
        mousePos.x += Cell.TileOffsetX;
        mousePos.y += Cell.TileOffsetY;
        Vector3Int posInt = grid.LocalToCell(mousePos);
        Cell cell;

        if (mousePos.x >= 1 && mousePos.y >= 1 &&
            mousePos.x < level.LevelSize.x && mousePos.y < level.LevelSize.y)
        {
            cell = level.GetCell((Vector2Int)posInt);
        }
        else
            cell = null;

        if (cell != null)
        {
            Vector3 crosshairPos = new Vector3(cell.Position.x, cell.Position.y, 0);
            crosshair.transform.position = crosshairPos;
        }

        if (Input.GetMouseButtonDown(0) &&
            TurnController.instance.gameState == GameState.Player0Turn)
        {
            path = level.pf.GetCellPath(level._player.Position, cell.Position);
            player.MoveAlongPath(path);
        }
        else if (Input.GetMouseButtonDown(1))
            GameLog.Send(cell.ToString(), MessageColour.White);
        else if (Input.GetMouseButtonDown(2))
            DebugChangeCell(cell);
    }

    // Gizmo vars
    List<Cell> path;

    // Debug function to change a floor to a wall and vice-versa
    void DebugChangeCell(Cell cell)
    {
        if (cell.Blocked)
        {
            cell.Blocked = false;
            cell.Opaque = false;
        }
        else
        {
            cell.Blocked = true;
            cell.Opaque = true;
        }
        level.RefreshFOV();
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            foreach (Cell c in path)
            {
                Gizmos.DrawWireCube(Helpers.GridToVector3(c.Position), new Vector3(.5f, .5f, .5f));
            }
        }

    }
}
