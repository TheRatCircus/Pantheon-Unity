// Corpse item
using UnityEngine;

[CreateAssetMenu(fileName = "New Corpse", menuName = "Items/Corpse")]
public class Corpse : ItemData
{
    public CorpseType _corpseType;

    public override void OnUse(Actor actor) { }
}
