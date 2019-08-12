// Base class for flasks
using UnityEngine;

[CreateAssetMenu(fileName = "New Flask", menuName = "Items/Flask")]
public class Flask : ItemData
{
    public FlaskType _flaskType;

    // Behaviour when used by a player
    public override void OnUse(Player user, Item item)
    {
        new ItemUseResult(user, item, onUse).TryAction();
    }

    // Behaviour when used by an NPC
    public override void OnUse(Enemy user, Item item)
    {
        throw new System.NotImplementedException("Enemy tried to use a flask");
    }
}
