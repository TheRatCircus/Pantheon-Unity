// Handler for player input to character
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Result of input
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
    List<Cell> targetLine;

    // Status
    InputState inputState = InputState.Move;

    // Properties
    public InputState _inputState
    {
        get => inputState;
        set => inputState = value;
    }

    // Events
    public event Action<Cell> PointTargetConfirmEvent;
    public event Action TargetConfirmEvent;
    public event Action TargetCancelEvent;

    // Start is called before the first frame update
    private void Start()
    {
        player = GetComponent<Player>();

        if (player != null)
            if (player._cell != null)
                MoveCrosshair(player._cell);
            else
                Debug.LogException(new Exception("Player was initialized without a cell"));
        else
            Debug.LogException(new Exception("No player found"));

        targetLine = new List<Cell>();
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
                {
                    PointTargetConfirmEvent?.Invoke(targetCell);
                    inputState = InputState.Move;
                }
                else if (Input.GetButtonDown("Cancel"))
                {
                    TargetCancelEvent?.Invoke();
                    inputState = InputState.Move;
                    GameLog.Send("Targetting cancelled.", MessageColour.Teal);
                }
            }
            else if (inputState == InputState.LineTarget)
            {
                if (Input.GetButtonDown("Up"))
                {
                    MoveCrosshair(player.level.GetAdjacentCell(targetCell, Vector2Int.up));
                    CellDrawer.PaintCells(player.level, targetLine);
                }
                else if (Input.GetButtonDown("Down"))
                {
                    MoveCrosshair(player.level.GetAdjacentCell(targetCell, Vector2Int.down));
                    CellDrawer.PaintCells(player.level, targetLine);
                }
                else if (Input.GetButtonDown("Left"))
                {
                    MoveCrosshair(player.level.GetAdjacentCell(targetCell, Vector2Int.left));
                    CellDrawer.PaintCells(player.level, targetLine);
                }
                else if (Input.GetButtonDown("Right"))
                {
                    MoveCrosshair(player.level.GetAdjacentCell(targetCell, Vector2Int.right));
                    CellDrawer.PaintCells(player.level, targetLine);
                }
                else if (Input.GetButtonDown("Up Left"))
                {
                    MoveCrosshair(player.level.GetAdjacentCell(targetCell, new Vector2Int(-1, 1)));
                    CellDrawer.PaintCells(player.level, targetLine);
                }
                else if (Input.GetButtonDown("Up Right"))
                {
                    MoveCrosshair(player.level.GetAdjacentCell(targetCell, new Vector2Int(1, 1)));
                    CellDrawer.PaintCells(player.level, targetLine);
                }
                else if (Input.GetButtonDown("Down Left"))
                {
                    MoveCrosshair(player.level.GetAdjacentCell(targetCell, new Vector2Int(-1, -1)));
                    CellDrawer.PaintCells(player.level, targetLine);
                }
                else if (Input.GetButtonDown("Down Right"))
                {
                    MoveCrosshair(player.level.GetAdjacentCell(targetCell, new Vector2Int(1, -1)));
                    CellDrawer.PaintCells(player.level, targetLine);
                }
                else if (Input.GetButtonDown("Submit"))
                {
                    ConfirmLineTargetting();
                }
                else if (Input.GetButtonDown("Cancel"))
                {
                    CancelLineTargetting();
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
        {
            MoveCrosshair(player.level.GetCell((Vector2Int)posInt));
            targetLine = Bresenhams.GetPath(player.level, player._cell, targetCell);
            switch (inputState)
            {
                case InputState.Move:
                    break;
                case InputState.LineTarget:
                    CellDrawer.PaintCells(player.level, Bresenhams.GetPath(player.level, player._cell, targetCell));
                    break;
            }
        }
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
            case InputState.LineTarget:
                if (Input.GetMouseButtonDown(0) && targetCell.Revealed)
                {
                    ConfirmLineTargetting();
                }
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
        if (cell._terrainData._terrainType == TerrainType.TerrainStoneWall)
            cell.SetTerrainType(Database.GetTerrain(TerrainType.TerrainStoneFloor));
        else if (cell._terrainData._terrainType == TerrainType.TerrainStoneFloor)
            cell.SetTerrainType(Database.GetTerrain(TerrainType.TerrainStoneWall));
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

    // Perform a line-targetted action. Return true if confirmed; false if
    // cancelled
    public IEnumerator LineTarget(ActionResult ActionResult)
    {
        // Start the line targetting input state
        inputState = InputState.LineTarget;
        bool confirmed = false;

        TargetConfirmEvent += () => confirmed = true;

        // When user sends confirm/cancel input, continue coroutine
        yield return new WaitUntil(() => inputState != InputState.LineTarget);
        if (confirmed)
            ActionResult.DoLineAction(targetLine);

        TargetConfirmEvent -= () => confirmed = true;
    }

    // Confirm target line
    private void ConfirmLineTargetting()
    {
        TargetConfirmEvent?.Invoke();
        inputState = InputState.Move;
        CellDrawer.UnpaintCells(player.level);
    }

    // Cancel line targetting process
    private void CancelLineTargetting()
    {
        TargetCancelEvent?.Invoke();
        inputState = InputState.Move;
        GameLog.Send("Targetting cancelled.", MessageColour.Teal);
        CellDrawer.UnpaintCells(player.level);
    }
}