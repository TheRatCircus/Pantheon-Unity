// ScrollData.cs
// Jerome Martina

using UnityEngine;

/// <summary>
/// Template for a scroll.
/// </summary>
[CreateAssetMenu(fileName = "New Scroll", menuName = "Items/Scroll")]
public class ScrollData : ItemData
{
    [SerializeField] private ScrollType scrollType = ScrollType.None;

    public ScrollType ScrollType { get => scrollType; }
}
