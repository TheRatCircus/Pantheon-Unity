// WeaponData.cs
// Jerome Martina

using UnityEngine;

/// <summary>
/// Template for a weapon.
/// </summary>
[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon",
    order = -1)]
public class WeaponData : ItemData
{
    [SerializeField] private WeaponType type;

    public WeaponType Type { get => Type; }
}
