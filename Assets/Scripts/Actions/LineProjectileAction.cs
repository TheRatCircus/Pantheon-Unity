// Creates and initializes a projectile following a line
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LineProjectileAction", menuName = "Actions/Line Projectile")]
public class LineProjectileAction : BaseAction
{
    public GameObject projPrefab;

    // Line-based action
    public override void DoAction(List<Cell> line)
    {
        MakeEntity.MakeLineProjectile(projPrefab, line);
    }
}
