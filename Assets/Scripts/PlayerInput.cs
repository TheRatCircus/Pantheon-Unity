// Handler for player input to character

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.World;
using Pantheon.Actions;
using Pantheon.Utils;

// Result of input
public enum InputState
{
    Move,
    PointTarget,
    LineTarget,
    Modal
}

public enum ModalListOperation
{
    Wield
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

    public List<Cell> TargetLine { get => targetLine; }
    public InputState InputState { get => inputState; set => inputState = value; }

    // Delegates
    public delegate void LineTargetDelegate();

    // Events
    public event Action<Cell> PointTargetConfirmEvent;
    public event Action TargetConfirmEvent;
    public event Action TargetCancelEvent;

    public event Action ModalConfirmEvent;
    public event Action ModalCancelEvent;

    public event Action<ModalListOperation> ModalListOpenEvent;

    // Start is called before the first frame update
    private void Start()
    {
        player = GetComponent<Player>();

        if (player != null)
            if (player.Cell != null)
                MoveCrosshair(player.Cell);
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

        if (Input.GetAxis("MouseX") != 0 || Input.GetAxis("MouseY") != 0)
            TargetCellByMouse();

        MouseInput();
    }

    public void Act()
    {
        // Actions only available on player's turn, but consume no energy
        if (InputState == InputState.Move)
            if (Input.GetButtonDown("AdvancedAttack"))
            {
                GameLog.Send("Advanced attack: select a target.", MessageColour.Teal);
                InputState = InputState.PointTarget;
            }
    }

    // Handle keyboard input feasible when not player's turn
    private void KeyInput()
    {
        if (InputState == InputState.Move)
        {
            if (Input.GetButtonDown("Up"))
                player.nextAction = new MoveAction(player, player.moveSpeed, Vector2Int.up);
            else if (Input.GetButtonDown("Down"))
                player.nextAction = new MoveAction(player, player.moveSpeed, Vector2Int.down);
            else if (Input.GetButtonDown("Left"))
                player.nextAction = new MoveAction(player, player.moveSpeed, Vector2Int.left);
            else if (Input.GetButtonDown("Right"))
                player.nextAction = new MoveAction(player, player.moveSpeed, Vector2Int.right);
            else if (Input.GetButtonDown("Up Left"))
                player.nextAction = new MoveAction(player, player.moveSpeed, new Vector2Int(-1, 1));
            else if (Input.GetButtonDown("Up Right"))
                player.nextAction = new MoveAction(player, player.moveSpeed, new Vector2Int(1, 1));
            else if (Input.GetButtonDown("Down Left"))
                player.nextAction = new MoveAction(player, player.moveSpeed, new Vector2Int(-1, -1));
            else if (Input.GetButtonDown("Down Right"))
                player.nextAction = new MoveAction(player, player.moveSpeed, new Vector2Int(1, -1));
            else if (Input.GetButtonDown("Wait"))
                player.nextAction = new WaitAction(player);
            else if (Input.GetButtonDown("Pickup"))
                player.nextAction = new PickupAction(player, player.Cell);
            else if (Input.GetButtonDown("Interact"))
            {
                if (player.Cell.Connection != null)
                    player.Cell.Connection.Use(player);
                player.nextAction = new WaitAction(player);
            }
            else if (Input.GetButtonDown("Wield"))
            {
                ModalListOpenEvent?.Invoke(ModalListOperation.Wield);
                InputState = InputState.Modal;
            }
            else if (Input.GetButtonDown("Cancel"))
                Game.QuitGame();
        }
        else if (InputState == InputState.PointTarget)
        {
            if (Input.GetButtonDown("Up"))
                MoveCrosshair(player.level.GetAdjacentCell(player.Cell, Vector2Int.up));
            else if (Input.GetButtonDown("Down"))
                MoveCrosshair(player.level.GetAdjacentCell(player.Cell, Vector2Int.down));
            else if (Input.GetButtonDown("Left"))
                MoveCrosshair(player.level.GetAdjacentCell(player.Cell, Vector2Int.left));
            else if (Input.GetButtonDown("Right"))
                MoveCrosshair(player.level.GetAdjacentCell(player.Cell, Vector2Int.right));
            else if (Input.GetButtonDown("Up Left"))
                MoveCrosshair(player.level.GetAdjacentCell(player.Cell, new Vector2Int(-1, 1)));
            else if (Input.GetButtonDown("Up Right"))
                MoveCrosshair(player.level.GetAdjacentCell(player.Cell, new Vector2Int(1, 1)));
            else if (Input.GetButtonDown("Down Left"))
                MoveCrosshair(player.level.GetAdjacentCell(player.Cell, new Vector2Int(-1, -1)));
            else if (Input.GetButtonDown("Down Right"))
                MoveCrosshair(player.level.GetAdjacentCell(player.Cell, new Vector2Int(1, -1)));
            else if (Input.GetButtonDown("Submit"))
            {
                PointTargetConfirmEvent?.Invoke(targetCell);
                InputState = InputState.Move;
            }
            else if (Input.GetButtonDown("Cancel"))
            {
                TargetCancelEvent?.Invoke();
                InputState = InputState.Move;
                GameLog.Send("Targetting cancelled.", MessageColour.Teal);
            }
        }
        else if (InputState == InputState.LineTarget)
        {
            if (Input.GetButtonDown("Up"))
            {
                MoveCrosshair(player.level.GetAdjacentCell(targetCell, Vector2Int.up));
                targetLine = Bresenhams.GetPath(player.level, player.Cell, targetCell);
                CellDrawer.PaintCells(player.level, targetLine);
            }
            else if (Input.GetButtonDown("Down"))
            {
                MoveCrosshair(player.level.GetAdjacentCell(targetCell, Vector2Int.down));
                targetLine = Bresenhams.GetPath(player.level, player.Cell, targetCell);
                CellDrawer.PaintCells(player.level, targetLine);
            }
            else if (Input.GetButtonDown("Left"))
            {
                MoveCrosshair(player.level.GetAdjacentCell(targetCell, Vector2Int.left));
                targetLine = Bresenhams.GetPath(player.level, player.Cell, targetCell);
                CellDrawer.PaintCells(player.level, targetLine);
            }
            else if (Input.GetButtonDown("Right"))
            {
                MoveCrosshair(player.level.GetAdjacentCell(targetCell, Vector2Int.right));
                targetLine = Bresenhams.GetPath(player.level, player.Cell, targetCell);
                CellDrawer.PaintCells(player.level, targetLine);
            }
            else if (Input.GetButtonDown("Up Left"))
            {
                MoveCrosshair(player.level.GetAdjacentCell(targetCell, new Vector2Int(-1, 1)));
                targetLine = Bresenhams.GetPath(player.level, player.Cell, targetCell);
                CellDrawer.PaintCells(player.level, targetLine);
            }
            else if (Input.GetButtonDown("Up Right"))
            {
                MoveCrosshair(player.level.GetAdjacentCell(targetCell, new Vector2Int(1, 1)));
                targetLine = Bresenhams.GetPath(player.level, player.Cell, targetCell);
                CellDrawer.PaintCells(player.level, targetLine);
            }
            else if (Input.GetButtonDown("Down Left"))
            {
                MoveCrosshair(player.level.GetAdjacentCell(targetCell, new Vector2Int(-1, -1)));
                targetLine = Bresenhams.GetPath(player.level, player.Cell, targetCell);
                CellDrawer.PaintCells(player.level, targetLine);
            }
            else if (Input.GetButtonDown("Down Right"))
            {
                MoveCrosshair(player.level.GetAdjacentCell(targetCell, new Vector2Int(1, -1)));
                targetLine = Bresenhams.GetPath(player.level, player.Cell, targetCell);
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
        else if (InputState == InputState.Modal)
        {
            if (Input.GetButtonDown("Submit"))
            {
                ModalConfirmEvent?.Invoke();
                ModalConfirmEvent = null;
            }
            else if (Input.GetButtonDown("Cancel"))
            {
                ModalCancelEvent?.Invoke();
                ModalCancelEvent = null;
                InputState = InputState.Move;
            }
        }

        // Actions which are always available
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
            targetLine = Bresenhams.GetPath(player.level, player.Cell, targetCell);
            switch (InputState)
            {
                case InputState.Move:
                    break;
                case InputState.PointTarget:
                    break;
                case InputState.LineTarget:
                    CellDrawer.PaintCells(player.level, Bresenhams.GetPath(player.level, player.Cell, targetCell));
                    break;
            }
        }
    }

    // Send mouse commands other than crosshair move
    private void MouseInput()
    {
        switch (InputState)
        {
            // Left-click is contextual
            case InputState.Move:
                if (Input.GetMouseButtonDown(0) && targetCell.Revealed)
                {
                    // Don't do anything if player clicks on themselves
                    if (targetCell._actor is Player)
                        break;

                    path = player.level.pf.GetCellPath(Game.GetPlayer().Position, targetCell.Position);
                    player.MovePath = path;
                    //player.MoveAlongPath(path);
                }
                break;
            case InputState.PointTarget:
                break;
            case InputState.LineTarget:
                if (Input.GetMouseButtonDown(0) && targetCell.Revealed)
                    ConfirmLineTargetting();
                break;
        }

        // Finally, examine is always available
        if (Input.GetMouseButtonDown(1))
            GameLog.Send(targetCell.ToString(), MessageColour.White);
        else if (Input.GetMouseButtonDown(2))
            DebugChangeCell(targetCell);
    }

    // Move crosshair and change its targetted cell
    private void MoveCrosshair(Cell newCell)
    {
        targetCell = newCell;
        if (targetCell != null)
        {
            Vector3 crosshairPos = new Vector3(newCell.Position.x, newCell.Position.y);
            crosshair.transform.position = crosshairPos;
        }
        else
            Debug.LogWarning("Attempted to move crosshair to null cell");
    }

    // Debug function to change a floor to a wall and vice-versa
    private void DebugChangeCell(Cell cell)
    {
        if (cell.TerrainData._terrainType == TerrainType.StoneWall)
            cell.SetTerrainType(Database.GetTerrain(TerrainType.StoneFloor));
        else if (cell.TerrainData._terrainType == TerrainType.StoneFloor)
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

    // Perform a line-targetted action. Return true if confirmed; false if
    // cancelled
    public IEnumerator LineTarget(LineTargetDelegate onConfirm)
    {
        // Start the line targetting input state
        InputState = InputState.LineTarget;
        bool confirmed = false;

        TargetConfirmEvent += () => confirmed = true;

        // When user sends confirm/cancel input, continue
        yield return new WaitUntil(() => InputState != InputState.LineTarget);
        if (confirmed)
            onConfirm?.Invoke();

        TargetConfirmEvent -= () => confirmed = true;
    }

    // Confirm target line
    private void ConfirmLineTargetting()
    {
        TargetConfirmEvent?.Invoke();
        InputState = InputState.Move;
        CellDrawer.UnpaintCells(player.level);
    }

    // Cancel line targetting process
    private void CancelLineTargetting()
    {
        TargetCancelEvent?.Invoke();
        InputState = InputState.Move;
        GameLog.Send("Targetting cancelled.", MessageColour.Teal);
        CellDrawer.UnpaintCells(player.level);
    }
}
