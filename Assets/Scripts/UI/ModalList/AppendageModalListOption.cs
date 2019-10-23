// AppendageModalListOption.cs
// Jerome Martina

using UnityEngine;
using Pantheon.Actors;

namespace Pantheon.UI
{
    /// <summary>
    /// An option representing an appendage in a modal list.
    /// </summary>
    public class AppendageModalListOption : ModalListOption
    {
        [SerializeField] [ReadOnly] private Appendage appendage;
        public Appendage Appendage
        {
            get => appendage;
            set => appendage = value;
        }

        public delegate void OnClickDelegate(AppendageModalListOption option);
        OnClickDelegate onClick;

        public void Initialize(Appendage appendage, OnClickDelegate onClick)
        {
            Appendage = appendage;
            icon.sprite = appendage.Sprite;

            string optionText = $"{appendage.DisplayName}";
            if (appendage.Item != null)
                optionText += $" ({appendage.Item.DisplayName})";
            text.text = optionText;

            this.onClick = onClick;
        }

        public void OnClick() => onClick?.Invoke(this);
    }

}
