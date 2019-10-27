// Spell.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon
{
    [CreateAssetMenu(fileName = "New Spell",
        menuName = "Pantheon/Content/Spell")]
    public sealed class Spell : Content
    {
        [SerializeField] private string displayName = "DEFAULT_SPELL_NAME";
        [SerializeField] private Sprite sprite = null;

        [SerializeField] private ActionWrapper onCast = null;
        [SerializeField] private int castTime = -1;

        public string DisplayName => displayName;
        public Sprite Sprite => sprite;
        public ActionWrapper OnCast => onCast;
        public int CastTime => castTime;
    }
}
