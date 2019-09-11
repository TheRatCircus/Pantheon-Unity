// StatusFactory.cs
// Jerome Martina

using UnityEngine;
using Pantheon.Actors;
using Pantheon.Utils;

/// <summary>
/// Shortens status effect construction and guarantees consistency between
/// instances.
/// </summary>
public static class StatusFactory
{
    public static StatusEffect GetStatus(StatusType type)
    {
        switch (type)
        {
            case StatusType.Haste:
                return new StatusEffect(
                    "Haste", StatusType.Haste,
                    Strings.TextColour.Green,
                    ApplyHaste, ExpireHaste);
            default:
                throw new System.Exception("Bad StatusType passed.");
        }
    }

    public static string ApplyHaste(Actor actor)
    {
        actor.Speed = Mathf.FloorToInt(actor.Speed * 1.3f);

        string playerMsg = $"{Strings.GetSubject(actor, true)}" +
            $" feel yourself speed up!";
        string enemyMsg = $"{Strings.GetSubject(actor, true)}" +
            $" speeds up.";

        playerMsg = Strings.ColourString(playerMsg, Strings.TextColour.Green);
        enemyMsg = Strings.ColourString(enemyMsg, Strings.TextColour.White);

        if (actor is Player)
            return playerMsg;
        else
            return enemyMsg;
    }

    public static string ExpireHaste(Actor actor)
    {
        actor.Speed = Mathf.FloorToInt(actor.Speed / 1.3f);

        string playerMsg = $"{Strings.GetSubject(actor, true)}" +
                $" feel yourself slow down to a normal speed.";
        string enemyMsg = $"{Strings.GetSubject(actor, true)}" +
                $" slows down to a normal speed.";

        playerMsg = Strings.ColourString(playerMsg, Strings.TextColour.Blue);
        enemyMsg = Strings.ColourString(enemyMsg, Strings.TextColour.White);

        if (actor is Player)
            return playerMsg;
        else
            return enemyMsg;
    }
}
