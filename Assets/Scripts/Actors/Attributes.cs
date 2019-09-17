// Attribute.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.Actors
{
    [System.Serializable]
    public class Attributes
    {
        [SerializeField] private int strength;
        [SerializeField] private int dexterity;
        [SerializeField] private int intellect;
        [SerializeField] private int constitution;
        [SerializeField] private int willpower;
    }
}