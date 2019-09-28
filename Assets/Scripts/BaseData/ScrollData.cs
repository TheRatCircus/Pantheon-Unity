// ScrollData.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// Template for a scroll.
    /// </summary>
    [CreateAssetMenu(fileName = "New Scroll",
        menuName = "BaseData/Items/Scroll")]
    public class ScrollData : ItemData
    {
        [SerializeField] private ScrollType scrollType = ScrollType.None;

        public ScrollType ScrollType { get => scrollType; }
    }

    public enum ScrollType
    {
        None = 0,
        MagicBullet = 1
    }

}
