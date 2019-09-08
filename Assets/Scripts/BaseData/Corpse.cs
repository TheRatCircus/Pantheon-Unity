// Corpse.cs
// Jerome Martina

using UnityEngine;

/// <summary>
/// Template for a corpse.
/// </summary>
[CreateAssetMenu(fileName = "New Corpse", menuName = "Items/Corpse")]
public class Corpse : ItemData
{
    [SerializeField] private CorpseType corpseType = CorpseType.None;

    public CorpseType CorpseType { get => corpseType; }
}
