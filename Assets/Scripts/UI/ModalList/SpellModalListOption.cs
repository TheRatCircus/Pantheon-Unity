// SpellModalListOption.cs
// Jerome Martina

using UnityEngine;

namespace Pantheon.UI
{
    public class SpellModalListOption : ModalListOption
    {
        [SerializeField] [ReadOnly] private Spell spell;
        public Spell Spell { get => spell; set => spell = value; }

        public delegate void OnClickDelegate(SpellModalListOption option);
        OnClickDelegate onClick;

        public void Initialize(Spell spell, OnClickDelegate onClick)
        {
            Spell = spell;
            icon.sprite = spell.Sprite;
            text.text = spell.DisplayName;
            this.onClick = onClick;
        }

        public void OnClick()
        {
            LogModal($"Selecting option: {text.text}");
            onClick?.Invoke(this);
        }  

        [System.Diagnostics.Conditional("DEBUG_MODAL")]
        public void LogModal(string logMsg)
        {
            Debug.Log(logMsg);
        }
    }
}
