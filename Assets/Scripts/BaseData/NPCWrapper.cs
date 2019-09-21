// NPCWrapper.cs
// Jerome Martina

using UnityEngine;

/// <summary>
/// NPC prefab with an enumeration.
/// </summary>
[CreateAssetMenu(fileName = "New NPCWrapper", menuName = "BaseData/NPCWrapper")]
public class NPCWrapper : ScriptableObject
{
    // Prefabs cannot be enumerated for the Database without this
    [SerializeField] private NPCType type = NPCType.None;
    [SerializeField] private GameObject prefab = null;

    public NPCType Type { get => type;}
    public GameObject Prefab { get => prefab; }
}