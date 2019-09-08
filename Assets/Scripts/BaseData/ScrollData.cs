// Base class for a scroll
using UnityEngine;

[CreateAssetMenu(fileName = "New Scroll", menuName = "Items/Scroll/Scroll")]
public class ScrollData : ItemData
{
    [SerializeField] private ScrollType scrollType = ScrollType.None;

    public ScrollType ScrollType { get => scrollType; }
}
