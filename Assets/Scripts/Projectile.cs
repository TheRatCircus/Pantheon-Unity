// Projectile.cs
// Jerome Martina

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pantheon.Core;
using Pantheon.World;
using Pantheon.Utils;

public class Projectile : MonoBehaviour
{
    public string ProjectileName;
    public List<Cell> ProjectileLine;

    public void FireProjectile()
    {
        StartCoroutine(Travel());
    }

    // Move this projectile along a line
    public IEnumerator Travel()
    {
        Game.instance.Lock();
        int i = 0;
        if (ProjectileLine.Count == 1)
        {
            // Proj was instantiated at its destination, do nothing here
        }
        else if (ProjectileLine.Count <= 0)
        {
            Debug.LogException(
                new System.Exception(
                    "A projectile attempted to follow a null path"));
        }
        else
        {
            // Iterate through every cell in path except last
            for (; i < ProjectileLine.Count - 1; i++)
            {
                yield return new WaitForSeconds(.02f);
                if (!ProjectileLine[i + 1].IsWalkableTerrain()
                    || ProjectileLine[i + 1].Actor != null)
                {
                    i++;
                    break;
                }
                else
                    transform.position = Helpers.V2IToV3(ProjectileLine[i + 1].Position);
            }
        }

        if (ProjectileLine[i].Actor != null)
        {
            GameLog.Send($"The magic bullet hits {GameLog.GetSubject(ProjectileLine[i].Actor, false)}!", MessageColour.White);
            ProjectileLine[i].Actor.TakeDamage(3);
        }
        Destroy(gameObject);
        Game.instance.Unlock();
    }
}
