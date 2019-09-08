// Melee.cs
// Jerome Martina

using UnityEngine;

/// <summary>
/// Stats applied to an item when it is used in melee.
/// </summary>
[System.Serializable]
public class Melee
{
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private int accuracy; // 0...100
    [SerializeField] private int attackTime; // Expressed as energy cost, so lower is faster

    // Properties
    public int MinDamage { get => minDamage; private set => minDamage = value; }
    public int MaxDamage { get => maxDamage; private set => maxDamage = value; }
    public int Accuracy { get => accuracy; private set => accuracy = value; }
    public int AttackTime { get => attackTime; private set => attackTime = value; }
}
