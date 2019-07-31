// Player controller
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    private int fovRadius = 7;

    // Properties
    public int FOVRadius { get => fovRadius; }

    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        actorName = "Player";
        maxHealth = 10;
        minDamage = 2;
        maxDamage = 4;
        base.Awake();
    }

    // Update is called once per frame
    private void Update()
    {
        CatchInput();
    }

    // Receive input from the player on the player's turn
    private void CatchInput()
    {
        if (TurnController.instance.gameState == GameState.Player0Turn)
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
                TurnController.instance.ChangeTurn();
        }
    }

    // Attempt to move along a path given a destination
    public void MoveAlongPath(Vector2Int targetPos)
    {
        List<Cell> path = level.pf.GetCellPath(Position, targetPos);
        foreach (Cell cell in path)
        {
            PlayerMove(cell);
        }
    }

    // Attempt to move along a given path
    public void MoveAlongPath(List<Cell> path)
    {
        foreach (Cell cell in path)
        {
            bool nearbyEnemy = false;
            PlayerMove(cell);
            foreach (Actor actor in level.actors)
                if (actor is Enemy && actor._Cell.Visible)
                    nearbyEnemy = true;
            if (nearbyEnemy)
            {
                GameLog.Send($"An enemy is nearby!", MessageColour.Red);
                break;
            }
        }
    }

    // Attempt to move to another cell by Vector
    protected override void TryMove(Vector2Int move)
    {
        Cell destinationCell;
        Vector2Int destinationPos =
            new Vector2Int((int)transform.position.x + move.x, (int)transform.position.y + move.y);
        destinationCell = level.Cells[destinationPos.x, destinationPos.y];
        if (destinationCell != null && destinationCell.IsWalkable())
            PlayerMove(destinationCell);
        else if (destinationCell._Actor != null)
            Attack(destinationCell);
    }

    // Actually make the move to another cell
    private void PlayerMove(Cell destinationCell)
    {
        MoveToCell(destinationCell);
        level.RefreshFOV();
        TurnController.instance.ChangeTurn();
    }

    // Handle the player's death
    protected override void OnDeath()
    {
        TurnController.instance.gameState = GameState.PlayersDead;
        GameLog.Send("You have expired...", MessageColour.Purple);
    }
}
