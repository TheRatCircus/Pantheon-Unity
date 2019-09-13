// Spell.cs
// Jerome Martina

using UnityEngine;
using Pantheon.Actions;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell")]
public class Spell : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private SpellType type;

    [SerializeField] private ActionWrapper onCast;
    [SerializeField] private int castTime;

    public string DisplayName { get => displayName; private set => displayName = value; }
    public SpellType Type { get => type; private set => type = value; }
    public ActionWrapper OnCast { get => onCast; private set => onCast = value; }
    public int CastTime { get => castTime; private set => castTime = value; }
}