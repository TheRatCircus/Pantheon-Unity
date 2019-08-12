// Corpse item
using UnityEngine;

[CreateAssetMenu(fileName = "New Corpse", menuName = "Items/Corpse")]
public class Corpse : ItemData
{
    public CorpseType _corpseType;

    // Behaviour when used by a player
    public override void OnUse(Player player, Item item)
    {
        throw new System.NotImplementedException("This item should not be usable.");
    }

    // Behaviour when used by an NPC
    public override void OnUse(Enemy enemy, Item item)
    {
        throw new System.NotImplementedException("This item should not be usable.");
    }
}
