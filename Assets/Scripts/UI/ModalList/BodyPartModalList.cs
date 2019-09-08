// BodyPartModalList.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using Pantheon.Actors;

namespace Pantheon.UI
{
    /// <summary>
    /// A modal list showing a set of body parts.
    /// </summary>
    public class BodyPartModalList : ModalList
    {
        // Parameters
        [ReadOnly] private Actor actor;

        // Status
        [ReadOnly]
        private List<BodyPartModalListOption> selected =
            new List<BodyPartModalListOption>();

        // Callback
        public delegate void SubmitPartsDelegate(List<BodyPart> parts);
        SubmitPartsDelegate onSubmit;

        public void Initialize(string prompt, Actor actor, int maxOptions,
            SubmitPartsDelegate onSubmit)
        {
            promptText.text = prompt;
            this.actor = actor;
            this.maxOptions = maxOptions;
            this.onSubmit = onSubmit;

            List<BodyPart> prehensiles = actor.GetPrehensiles();

            for (int i = 0; i < prehensiles.Count; i++)
            {
                GameObject optionObj = Instantiate(optionPrefab, listTransform);
                BodyPartModalListOption option = optionObj.GetComponent<BodyPartModalListOption>();
                option.Initialize(prehensiles[i], SelectPart);
            }
        }

        public void SelectPart(BodyPartModalListOption option)
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
            List<BodyPart> selectedParts = new List<BodyPart>();
            foreach (BodyPartModalListOption option in selected)
                selectedParts.Add(option.Part);

            onSubmit?.Invoke(selectedParts);
            selected.Clear();
            Clean();
        }
    }
}
