// FlyingProjectile.cs
// Jerome Martina

using System.Collections;
using UnityEngine;
using Pantheon.Core;
using Pantheon.World;
using Pantheon.Actions;
using Pantheon.Utils;

public class FlyingProjectile : MonoBehaviour
{
    public Cell TargetCell { get; set; }
    public bool Spins { get; set; }
    private Vector3 targetPos;

    // What happens when the projectile lands?
    public BaseAction OnLandAction;

    private void Start()
    {
        switch(OnLandAction)
        {
            case ExplodeAction a:
                a.Origin = TargetCell;
                break;
            case null:
                break;
            default:
                throw new System.Exception
                    ($"{OnLandAction.GetType()} cannot be handled.");
        }
        targetPos = Helpers.V2IToV3(TargetCell.Position);
        StartCoroutine(Fly());
    }

    private IEnumerator Fly()
    {
        Game.instance.Lock();
        while (transform.position != targetPos)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, targetPos, 1.5f);

            if (Spins)
                transform.Rotate(0, 0, 16, Space.Self);

            yield return new WaitForSeconds(.01f);
        }
        OnLandAction?.DoAction();
        Game.instance.Unlock();
        Destroy(gameObject);
    }
}
