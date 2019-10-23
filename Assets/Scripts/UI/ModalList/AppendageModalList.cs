// AppendageModalList.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using Pantheon.Actors;

namespace Pantheon.UI
{
    /// <summary>
    /// A modal list showing a set of appendages.
    /// </summary>
    public class AppendageModalList : ModalList
    {
        // Parameters
        [SerializeField] [ReadOnly] private Actor actor;

        // Status
        [SerializeField] [ReadOnly]
        private List<AppendageModalListOption> selected =
            new List<AppendageModalListOption>();

        // Callback
        public delegate void SubmitAppendagesDelegate(List<Appendage> appendages);
        SubmitAppendagesDelegate onSubmit;

        public void Initialize(string prompt, Actor actor, int maxOptions,
            SubmitAppendagesDelegate onSubmit)
        {
            promptText.text = prompt;
            this.actor = actor;
            this.maxOptions = maxOptions;
            this.onSubmit = onSubmit;

            List<Appendage> prehensiles = actor.Body.GetPrehensiles();

            for (int i = 0; i < prehensiles.Count; i++)
            {
                GameObject optionObj = Instantiate(optionPrefab, listTransform);
                AppendageModalListOption option
                    = optionObj.GetComponent<AppendageModalListOption>();
                option.Initialize(prehensiles[i], SelectAppendage);
            }
        }

        public void SelectAppendage(AppendageModalListOption option)
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

        /// <summary>
        /// Send the selected options to the passer of the callback.
        /// </summary>
        public void Submit()
        {
            List<Appendage> selectedAppendages = new List<Appendage>();
            foreach (AppendageModalListOption option in selected)
                selectedAppendages.Add(option.Appendage);

            onSubmit?.Invoke(selectedAppendages);
            selected.Clear();
            Clean();
        }
    }
}
