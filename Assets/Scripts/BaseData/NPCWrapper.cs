// NPCWrapper.cs
// Jerome Martina

using UnityEngine;

/// <summary>
/// NPC prefab with an enumeration.
/// </summary>
[CreateAssetMenu(fileName = "New NPCWrapper", menuName = "NPCWrapper")]
public class NPCWrapper : ScriptableObject
{
    // Prefabs cannot be enumerated for the Database without this
    public NPCType Type;
    public GameObject Prefab;
}