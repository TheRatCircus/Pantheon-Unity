// Scroll which fires a projectile
using UnityEngine;
using Pantheon.Actions;

[CreateAssetMenu(fileName = "New ProjectileScroll", menuName = "Items/Scroll/ProjectileScroll")]
public class ProjectileScroll : Scroll
{
    public GameObject projPrefab;

    // Behaviour when used by a player
    public override void OnUse(Player user, Item item)
    {
        ItemUseAction itemUse = new ItemUseAction(user, item);
        itemUse.onUse = new LineProjAction(itemUse.actor, projPrefab, itemUse.AssignAction);
    }

    // Behaviour when used by an NPC
    public override void OnUse(Enemy user, Item item)
    {
        throw new System.NotImplementedException("Enemy tried to use a scroll");
    }
}