// BodyPartModalListOption.cs
// Jerome Martina

using Pantheon.Actors;

namespace Pantheon.UI
{
    /// <summary>
    /// An option representing a body part in a modal list.
    /// </summary>
    public class BodyPartModalListOption : ModalListOption
    {
        public BodyPart part;

        public delegate void OnClickDelegate(BodyPartModalListOption option);
        OnClickDelegate onClick;

        public void Initialize(BodyPart part, OnClickDelegate onClick)
        {
            this.part = part;
            icon.sprite = part.Sprite;

            string optionText = $"{part.Name}";
            if (part.Item != null)
                optionText += $" ({part.Item.DisplayName})";
            text.text = optionText;

            this.onClick = onClick;
        }

        public void OnClick() => onClick?.Invoke(this);
    }

}
