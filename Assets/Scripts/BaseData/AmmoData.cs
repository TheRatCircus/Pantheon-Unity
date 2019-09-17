// AmmoData.cs
// Jerome Martina

using UnityEngine;

[CreateAssetMenu(fileName = "New Ammo", menuName = "Items/Ammo")]
public class AmmoData : ItemData
{
    [SerializeField] private AmmoType ammoType = AmmoType.None;

    public AmmoType AmmoType { get => ammoType; }
}

public enum AmmoType
{
    None,
    Cartridges
}