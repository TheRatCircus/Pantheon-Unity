// Generic projectile behaviour, played out at the end of a turn
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : EndTurnEffect
{
    public List<Cell> projectileLine;

    public void Awake()
    {
        Game.instance.EndTurnEffects.Add(this);
    }

    // Move this projectile along a line
    public override IEnumerator DoEffect()
    {
        int i = 0;
        if (projectileLine.Count == 1)
        {
            // Proj was instantiated at its destination, do nothing here
        }
        else if (projectileLine.Count <= 0)
        {
            Debug.LogException(
                new System.Exception(
                    "A projectile attempted to follow a null path"));
        }
        else
        {
            // Iterate through every cell in path except last
            for (; i < projectileLine.Count - 1; i++)
            {
                yield return new WaitForSeconds(.02f);
                if (!projectileLine[i + 1].IsWalkable())
                {
                    i++;
                    break;
                }
                else
                    transform.position = Helpers.V2IToV3(projectileLine[i + 1].Position);
            }
        }

        if (projectileLine[i]._actor != null)
        {
            GameLog.Send($"The magic bullet hits {GameLog.GetSubject(projectileLine[i]._actor, false)}!", MessageColour.White);
            projectileLine[i]._actor.Health -= 3;
        }
        Destroy(gameObject);
    }
}
