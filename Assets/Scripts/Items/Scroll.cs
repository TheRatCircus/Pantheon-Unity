// Base class for a scroll
using UnityEngine;

[CreateAssetMenu(fileName = "New Scroll", menuName = "Items/Scroll")]
public class Scroll : ItemData
{
    public ScrollType _scrollType;

    // Behaviour when used by a player
    public override void OnUse(Player user, Item item)
    {
        new ItemUseResult(user, item, onUse);
    }

    // Behaviour when used by an NPC
    public override void OnUse(Enemy user, Item item)
    {
        throw new System.NotImplementedException("Enemy tried to use a scroll");
    }
}
