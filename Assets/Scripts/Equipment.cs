// Equipment.cs
// Jerome Martina

using UnityEngine;

/// <summary>
/// Component of any item that can be equipped e.g. armour, jewellery.
/// </summary>
public class Equipment
{
    [SerializeField] private EquipType type = EquipType.None;

    // Armour, evasion, shielding
    // Resistances
    // ???

    public bool Equipable { get => type != EquipType.None; }
}