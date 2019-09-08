// ItemModalListOption.cs
// Jerome Martina

/// <summary>
/// An option representing an item in a modal list.
/// </summary>
public class ItemModalListOption : ModalListOption
{
    public Item Item;

    public delegate void OnClickDelegate(ItemModalListOption option);
    OnClickDelegate onClick;

    public void Initialize(Item item, OnClickDelegate onClick)
    {
        Item = item;
        icon.sprite = item._sprite;
        text.text = item.DisplayName;
        this.onClick = onClick;
    }

    public void OnClick() => onClick?.Invoke(this);
}
