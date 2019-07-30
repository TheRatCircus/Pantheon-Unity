// Enemy behaviour handling
using UnityEngine;

public class Enemy : Actor
{
    public Player player;
    Actor target;

    // Start is called before the first frame update
    void Start()
    {
        TurnController.turnController.OnEnemyTurnEvent += DecideAction;
    }

    // Evaluate the situation and decide what to do next
    void DecideAction()
    {
        if (cell.Visible)
        {
            target = player;
            Debug.DrawLine(Helpers.GridToVector3(Position), Helpers.GridToVector3(target.Position), Color.green, 10);
        }
    }
}
