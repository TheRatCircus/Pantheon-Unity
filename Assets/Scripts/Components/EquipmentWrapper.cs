// EquipmentWrapper.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Components
{
    [CreateAssetMenu(fileName = "New Equipment Component",
        menuName = "Pantheon/Components/Equipment")]
    public class EquipmentWrapper : ComponentWrapper
    {
        [SerializeField] private Equipment equipment = null;
        public override IComponent Get => equipment;
    }
}
