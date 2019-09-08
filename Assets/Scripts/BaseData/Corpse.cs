// Corpse item
using UnityEngine;

[CreateAssetMenu(fileName = "New Corpse", menuName = "Items/Corpse")]
public class Corpse : ItemData
{
    [SerializeField] private CorpseType corpseType = CorpseType.None;

    public CorpseType CorpseType { get => corpseType; }
}
