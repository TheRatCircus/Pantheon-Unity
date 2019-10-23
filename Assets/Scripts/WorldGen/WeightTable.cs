// ItemWeights.cs
// Jerome Martina

using UnityEngine;
using static Pantheon.Utils.RandomUtils;

namespace Pantheon.WorldGen
{
    [CreateAssetMenu(fileName = "New Weight Table",
        menuName = "Pantheon/WeightTable")]
    public sealed class WeightTable : ScriptableObject
    {
        [SerializeField] private SerializableRandomPick[] weights;
    }
}