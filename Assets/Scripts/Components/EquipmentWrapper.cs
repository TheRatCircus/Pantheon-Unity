// EquipmentWrapper.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Components
{
    [UnityEngine.CreateAssetMenu(fileName = "New Equipment Component",
        menuName = "BaseData/Components/Equipment")]
    public class EquipmentWrapper : ComponentWrapper
    {
        [SerializeField] private Equipment equipment = null;
        public override IComponent Get => equipment;
    }
}
