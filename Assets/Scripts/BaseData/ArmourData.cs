// ArmourData.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    [CreateAssetMenu(fileName = "New Armour",
        menuName = "BaseData/Items/Armour")]
    public sealed class ArmourData : ItemData
    {
        [SerializeField] private ArmourRef armourRef = ArmourRef.None;

        public ArmourRef ArmourRef => armourRef;
    }

    public enum ArmourRef
    {
        None,
        Cuirass
    }
}
