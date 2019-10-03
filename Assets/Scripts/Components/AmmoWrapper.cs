// AmmoWrapper.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Components
{
    [CreateAssetMenu(fileName = "New Ammo Component",
        menuName = "BaseData/Components/Ammo")]
    public class AmmoWrapper : ComponentWrapper
    {
        [SerializeField] private Ammo ammo = null;
        public override IComponent Get => ammo;
    }
}
