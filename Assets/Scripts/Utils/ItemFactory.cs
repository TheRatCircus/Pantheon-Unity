// ItemFactory.cs
// Jerome Martina

using static Pantheon.Core.Database;

namespace Pantheon.Utils
{
    /// <summary>
    /// Takes item types and returns new items to shorten calls.
    /// </summary>
    public static class ItemFactory
    {
        public static Item NewFlask(FlaskType type)
            => new Item(GetFlask(type));

        public static Item NewScroll(ScrollType type)
            => new Item(GetScroll(type));

        public static Item NewWeapon(WeaponType type)
            => new Item(GetWeapon(type));

        public static Item NewAmmo(AmmoType type)
            => new Item(GetAmmo(type));
    }
}
