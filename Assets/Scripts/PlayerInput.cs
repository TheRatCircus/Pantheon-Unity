// Handler for player input to character
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InputState
{
    Move,
    PointTarget,
    LineTarget
}

public class PlayerInput : MonoBehaviour
{
    // Requisite objects
    Player player;
    public Grid grid;
    public Image crosshair;
    Cell targetCell;

    // Status
    InputState inputState = InputState.Move;

    // Events
    public event Action<Cell> OnTargetConfirmEvent;
    public event Action OnTargetCancelEvent;

    // Start is called before the first frame update
    private void Start()
    {
        player = GetComponent<Player>();

        if (player != null)
        {
            if (player._cell != null)
            {
                MoveCrosshair(player._cell);
            }
            else
                Debug.LogException(new Exception("Player was initialized without a cell"));
        }
        else
            Debug.LogException(new Exception("No player found"));
    }

    // Update is called once per frame
    void Update()
    {
        KeyInput();
        if (Input.GetAxis("MouseX") != 0
            || Input.GetAxis("MouseY") != 0)
        {
            TargetCellByMouse();
        }
        MouseInput();
    }

    // Handle keyboard input
    private void KeyInput()
    {
        if (TurnController.instance.gameState == GameState.Player0Turn)
        {
            if (inputState == InputState.Move)
            {
                if (Input.GetButtonDown("Up"))
                    player.PlayerTryMove(Vector2Int.up);
                else if (Input.GetButtonDown("Down"))
                    player.PlayerTryMove(Vector2Int.down);
                else if (Input.GetButtonDown("Left"))
                    player.PlayerTryMove(Vector2Int.left);
                else if (Input.GetButtonDown("Right"))
                    player.PlayerTryMove(Vector2Int.right);
                else if (Input.GetButtonDown("Up Left"))
                    player.PlayerTryMove(new Vector2Int(-1, 1));
                else if (Input.GetButtonDown("Up Right"))
                    player.PlayerTryMove(new Vector2Int(1, 1));
                else if (Input.GetButtonDown("Down Left"))
                    player.PlayerTryMove(new Vector2Int(-1, -1));
                else if (Input.GetButtonDown("Down Right"))
                    player.PlayerTryMove(new Vector2Int(1, -1));
                else if (Input.GetButtonDown("Wait"))
                    player.WaitOneTurn();
                else if (Input.GetButtonDown("Pickup"))
                    player.TryPickup();
                else if (Input.GetButtonDown("AdvancedAttack"))
                {
                    GameLog.Send("Advanced attack: select a target.", MessageColour.Teal);
                    inputState = InputState.PointTarget;
                }
            }
            else if (inputState == InputState.PointTarget)
            {
                if (Input.GetButtonDown("Up"))
                    MoveCrosshair(player.level.GetAdjacentCell(player._cell, Vector2Int.up));
                else if (Input.GetButtonDown("Down"))
                    MoveCrosshair(player.level.GetAdjacentCell(player._cell, Vector2Int.down));
                else if (Input.GetButtonDown("Left"))
                    MoveCrosshair(player.level.GetAdjacentCell(player._cell, Vector2Int.left));
                else if (Input.GetButtonDown("Right"))
                    MoveCrosshair(player.level.GetAdjacentCell(player._cell, Vector2Int.right));
                else if (Input.GetButtonDown("Up Left"))
                    MoveCrosshair(player.level.GetAdjacentCell(player._cell, new Vector2Int(-1, 1)));
                else if (Input.GetButtonDown("Up Right"))
                    MoveCrosshair(player.level.GetAdjacentCell(player._cell, new Vector2Int(1, 1)));
                else if (Input.GetButtonDown("Down Left"))
                    MoveCrosshair(player.level.GetAdjacentCell(player._cell, new Vector2Int(-1, -1)));
                else if (Input.GetButtonDown("Down Right"))
                    MoveCrosshair(player.level.GetAdjacentCell(player._cell, new Vector2Int(1, -1)));
                else if (Input.GetButtonDown("Submit"))
                    OnTargetConfirmEvent?.Invoke(targetCell);
                else if (Input.GetButtonDown("Cancel"))
                {
                    OnTargetCancelEvent?.Invoke();
                    inputState = InputState.Move;
                    GameLog.Send("Targetting cancelled.", MessageColour.Teal);
                }
            }
        }
        
        // Actions feasible even if not this player's turn
        if (Input.GetButtonDown("Inventory"))
            player.RaiseInventoryToggleEvent();
    }

    #region Mouse

    // Move crosshair by mouse
    private void TargetCellByMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Offset tile anchor
        mousePos.x += Cell.TileOffsetX;
        mousePos.y += Cell.TileOffsetY;
        Vector3Int posInt = grid.LocalToCell(mousePos);

        if (player.level.Contains((Vector2Int)posInt))
            MoveCrosshair(player.level.GetCell((Vector2Int)posInt));
        else
            targetCell = null;
    }

    // Send mouse commands other than crosshair move
    private void MouseInput()
    {
        switch (inputState)
        {
            // Left-click is contextual
            case InputState.Move:
                if (Input.GetMouseButtonDown(0) &&
                TurnController.IsPlayerTurn() &&
                targetCell.Revealed)
                {
                    path = player.level.pf.GetCellPath(player.level._player.Position, targetCell.Position);
                    player.MoveAlongPath(path);
                }
                break;
            case InputState.PointTarget:
                break;
        }

        // Finally, examine is always available
        if (Input.GetMouseButtonDown(1))
            GameLog.Send(targetCell.ToString(), MessageColour.White);
        else if (Input.GetMouseButtonDown(2))
            DebugChangeCell(targetCell);
    }

    private void MoveCrosshair(Cell newCell)
    {
        targetCell = newCell;
        if (targetCell != null)
        {
            // Move crosshair
            Vector3 crosshairPos = new Vector3(newCell.Position.x, newCell.Position.y);
            crosshair.transform.position = crosshairPos;
        }
        else
            Debug.LogWarning("Attempted to move crosshair to null cell");
    }

    // Debug function to change a floor to a wall and vice-versa
    private void DebugChangeCell(Cell cell)
    {
        if (cell._terrainData._terrainType == TerrainType.StoneWall)
            cell.SetTerrainType(Database.GetTerrain(TerrainType.StoneFloor));
        else if (cell._terrainData._terrainType == TerrainType.StoneFloor)
            cell.SetTerrainType(Database.GetTerrain(TerrainType.StoneWall));
        player.level.RefreshFOV();
    }

    // Pathfinding list, also used for gizmo
    List<Cell> path;

    private void OnDrawGizmos()
    {
        if (path != null)
            foreach (Cell c in path)
                Gizmos.DrawWireCube(Helpers.V2IToV3(c.Position), new Vector3(.5f, .5f, .5f));
    }

    #endregion
}
