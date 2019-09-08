// WeaponData.cs
// Jerome Martina

/// <summary>
/// Template data for a weapon.
/// </summary>
[UnityEngine.CreateAssetMenu(
    fileName = "New Weapon",
    menuName = "Items/Weapon",
    order = -1)]
public class WeaponData : ItemData
{
    public WeaponType Type;
}
