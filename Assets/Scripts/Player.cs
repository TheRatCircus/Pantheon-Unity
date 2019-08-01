// Player controller
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
            if (Input.GetButtonDown("Up"))
                PlayerTryMove(new Vector2Int(0, 1));
            else if (Input.GetButtonDown("Down"))
                PlayerTryMove(new Vector2Int(0, -1));
            else if (Input.GetButtonDown("Left"))
                PlayerTryMove(new Vector2Int(-1, 0));
            else if (Input.GetButtonDown("Right"))
                PlayerTryMove(new Vector2Int(1, 0));
            else if (Input.GetButtonDown("Up Left"))
                PlayerTryMove(new Vector2Int(-1, 1));
            else if (Input.GetButtonDown("Up Right"))
                PlayerTryMove(new Vector2Int(1, 1));
            else if (Input.GetButtonDown("Down Left"))
                PlayerTryMove(new Vector2Int(-1, -1));
            else if (Input.GetButtonDown("Down Right"))
                PlayerTryMove(new Vector2Int(1, -1));
            else if (Input.GetButtonDown("Wait"))
                TurnController.instance.ChangeTurn();
        }
    }

    // Attempt to move along a given path
    public void MoveAlongPath(List<Cell> path)
    {
        foreach (Cell cell in path)
        {
            bool nearbyEnemy = false;
            foreach (Actor actor in level.actors)
                if (actor is Enemy && actor._Cell.Visible)
                    nearbyEnemy = true;
            if (nearbyEnemy)
            {
                GameLog.Send($"An enemy is nearby!", MessageColour.Red);
                break;
            }
            PlayerTryMove(cell);
        }
    }

    // Attempt to move to another given Cell
    private void PlayerTryMove(Cell cell)
    {
        if (cell != null && cell.IsWalkable())
            PlayerMove(cell);
        else if (cell._Actor != null)
            Attack(cell);
    }

    // Attempt to move to another cell by delta Vector
    private void PlayerTryMove(Vector2Int pos)
    {
        Cell cell = level.GetCell(this.cell.Position + pos);
        if (cell != null && cell.IsWalkable())
            PlayerMove(cell);
        else if (cell._Actor != null)
            Attack(cell);
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
