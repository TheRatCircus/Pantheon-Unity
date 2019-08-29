// Base class for flasks
using UnityEngine;
using Pantheon.Actions;

[CreateAssetMenu(fileName = "New Flask", menuName = "Items/Flask")]
public class Flask : ItemData
{
    public FlaskType _flaskType;

    // Behaviour when used by a player
    public override void OnUse(Player user, Item item)
    {
        ItemUseAction itemUse = new ItemUseAction(user, item, new HealAction(user, 0, .25f));
        itemUse.DoAction();
    }

    // Behaviour when used by an NPC
    public override void OnUse(Enemy user, Item item)
    {
        throw new System.NotImplementedException("Enemy tried to use a flask");
    }
}
