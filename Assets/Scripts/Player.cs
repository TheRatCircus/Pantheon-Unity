// Player controller
using UnityEngine;
using System.Collections.Generic;

public class Player : Actor
{
    private int fovRadius = 7;

    // Properties
    public int FOVRadius { get => fovRadius; }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        maxHealth = 100;
        health = maxHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        CatchInput();
    }

    // Receive input from the player on the player's turn
    private void CatchInput()
    {
        if (TurnController.turnController.gameState == GameState.Player0Turn)
        {
            // TODO: Clean this up
            if (Input.GetButtonDown("Up"))
                TryMove(new Vector2Int(0, 1));
            else if (Input.GetButtonDown("Down"))
                TryMove(new Vector2Int(0, -1));
            else if (Input.GetButtonDown("Left"))
                TryMove(new Vector2Int(-1, 0));
            else if (Input.GetButtonDown("Right"))
                TryMove(new Vector2Int(1, 0));
            else if (Input.GetButtonDown("Up Left"))
                TryMove(new Vector2Int(-1, 1));
            else if (Input.GetButtonDown("Up Right"))
                TryMove(new Vector2Int(1, 1));
            else if (Input.GetButtonDown("Down Left"))
                TryMove(new Vector2Int(-1, -1));
            else if (Input.GetButtonDown("Down Right"))
                TryMove(new Vector2Int(1, -1));
            else if (Input.GetButtonDown("Wait"))
                TurnController.turnController.ChangeTurn();
        }
    }

    // Attempt to move to another cell
    private void TryMove(Vector2Int move)
    {
        Cell destinationCell;
        Vector2Int destinationPos = 
            new Vector2Int((int)transform.position.x + move.x, (int)transform.position.y + move.y);
        destinationCell = currentLevel.Cells[destinationPos.x, destinationPos.y];
        if (destinationCell != null && destinationCell.IsWalkable())
            Move(destinationCell);
    }

    // Attempt to move along a path given a destination
    public void MoveAlongPath(Vector2Int targetPos)
    {
        List<Cell> path = currentLevel.pf.GetCellPath(Position, targetPos);
        foreach (Cell cell in path)
        {
            Move(cell);
        }
    }

    // Attempt to move along a given path
    public void MoveAlongPath(List<Cell> path)
    {
        foreach (Cell cell in path)
        {
            Move(cell);
        }
    }

    // Actually make the move to another cell
    private void Move(Cell destinationCell)
    {
        MoveToCell(destinationCell);
        currentLevel.RefreshFOV();
        TurnController.turnController.ChangeTurn();
    }
}
