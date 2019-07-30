// Enemy behaviour handling
public class Enemy : Actor
{
    // Start is called before the first frame update
    void Start()
    {
        TurnController.turnController.OnEnemyTurnEvent += DecideAction;
    }

    // Evaluate the situation and decide what to do next
    void DecideAction()
    {

    }
}
