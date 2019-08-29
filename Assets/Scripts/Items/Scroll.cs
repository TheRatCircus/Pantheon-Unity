// Base class for a scroll
using UnityEngine;
using Pantheon.Actions;

[CreateAssetMenu(fileName = "New Scroll", menuName = "Items/Scroll/BasicScroll")]
public class Scroll : ItemData
{
    public ScrollType _scrollType;

    // Behaviour when used by a player
    public override void OnUse(Player user, Item item)
    {
        ItemUseAction itemUse = new ItemUseAction(user, item);
    }

    // Behaviour when used by an NPC
    public override void OnUse(Enemy user, Item item)
    {
        throw new System.NotImplementedException("Enemy tried to use a scroll");
    }
}
