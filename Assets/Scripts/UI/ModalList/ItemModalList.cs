// ItemModalList.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A modal list showing a set of items.
/// </summary>
public class ItemModalList : ModalList
{
    // Parameters
    public Actor Actor; // Can be null

    // Status
    List<ItemModalListOption> selected = new List<ItemModalListOption>();

    // Callback
    public delegate void SubmitItemDelegate(Item item);
    SubmitItemDelegate onSubmit;

    /// <summary>
    /// Initialize this list with an actor's inventory.
    /// </summary>
    /// <param name="actor"></param>
    public void Initialize(string prompt, Actor actor, int maxOptions,
        SubmitItemDelegate onSubmit)
    {
        promptText.text = prompt;
        this.maxOptions = maxOptions;
        this.onSubmit = onSubmit;

        for (int i = 0; i < actor.Inventory.Count; i++)
        {
            GameObject optionObj = Instantiate(optionPrefab, listTransform);
            ItemModalListOption option = optionObj.GetComponent<ItemModalListOption>();
            option.Initialize(actor.Inventory[i], SelectItem);
        }
    }

    public void SelectItem(ItemModalListOption option)
    {
        if (selected.Count == maxOptions)
            return;

        if (maxOptions == 1)
        {
            selected.Add(option);
            Submit(); // Only one can be selected, so nothing more to do
            selected.Clear();
            return;
        }

        if (selected.Contains(option))
        {
            selected.Remove(option);
            option.SetSelected(false);
        }
        else
        {
            selected.Add(option);
            option.SetSelected(true);
        }
    }

    public void Submit()
    {
        onSubmit?.Invoke(selected[0].Item);
        selected.Clear();
        Clean();
    }
}
