// Healing action
using UnityEngine;

[CreateAssetMenu(fileName = "New HealAction", menuName = "Actions/Heal Action")]
public class HealAction : BaseAction
{
    public bool percentBased;
    public int healAmount;
    public float healPercent;

    // Carry out the healing action
    public override void DoAction(Actor actor)
    {
        if (percentBased)
            actor.Health += (int)(actor.MaxHealth * healPercent);
        else
            actor.Health += healAmount;

        if (actor is Player)
            GameLog.Send("You are rejuvenated!", MessageColour.White);
        else
            GameLog.Send($"The {actor.actorName} appears rejuvenated.", MessageColour.White);
    }
}
