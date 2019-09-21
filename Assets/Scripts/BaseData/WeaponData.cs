// WeaponData.cs
// Jerome Martina

using UnityEngine;

/// <summary>
/// Template for a weapon.
/// </summary>
[CreateAssetMenu(fileName = "New Weapon", menuName = "BaseData/Items/Weapon",
    order = -1)]
public class WeaponData : ItemData
{
    [SerializeField] private WeaponType type = WeaponType.None;

    public WeaponType Type { get => type; }
}
