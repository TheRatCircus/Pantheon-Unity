// RangedWrapper.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Components
{
    [CreateAssetMenu(fileName = "New Ranged Component",
        menuName = "Pantheon/Components/Ranged")]
    public class RangedWrapper : ComponentWrapper
    {
        [SerializeField] private Ranged ranged = null;
        public override IComponent Get => ranged;
    }
}
