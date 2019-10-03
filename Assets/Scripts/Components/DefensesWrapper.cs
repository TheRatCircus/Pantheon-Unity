// DefensesWrapper.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Components
{
    [CreateAssetMenu(fileName = "New Defenses Component",
        menuName = "BaseData/Components/Defenses")]
    public class DefensesWrapper : ComponentWrapper
    {
        [SerializeField] private Defenses defenses = null;
        public override IComponent Get => defenses;
    }
}