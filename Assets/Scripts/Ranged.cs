// Ranged.cs
// Jerome Martina

using UnityEngine;

[System.Serializable]
public class Ranged
{
    [SerializeField] private int minDamage = -1;
    [SerializeField] private int maxDamage = -1;
    [SerializeField] private int accuracy = -1;
    [SerializeField] private AmmoFamily ammoFamily;

    public int MinDamage { get => minDamage; set => minDamage = value; }
    public int MaxDamage { get => maxDamage; set => maxDamage = value; }
    public int Accuracy { get => accuracy; set => accuracy = value; }
    public AmmoFamily AmmoFamily { get => ammoFamily; set => ammoFamily = value; }
}