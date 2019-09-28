// WeaponData.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// Template for a weapon.
    /// </summary>
    [CreateAssetMenu(fileName = "New Weapon",
        menuName = "BaseData/Items/Weapon", order = -1)]
    public class WeaponData : ItemData
    {
        [SerializeField] private WeaponType type = WeaponType.None;

        public WeaponType Type { get => type; }
    }

    public enum WeaponType
    {
        None = 0,
        Dagger = 1,
        Hatchet = 2,
        Prejudice = 3,
        HandGrenade = 4,
        Carbine = 5,
    }
}
