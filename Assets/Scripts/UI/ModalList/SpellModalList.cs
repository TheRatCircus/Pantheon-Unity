// SpellModalList.cs
// Jerome Martina

using System.Collections.Generic;
using UnityEngine;
using Pantheon.Actors;

namespace Pantheon.UI
{
    public class SpellModalList : ModalList
    {
        // Status
        private List<SpellModalListOption> selected
            = new List<SpellModalListOption>();

        // Callback
        public delegate void SubmitSpellDelegate(Spell spell);
        SubmitSpellDelegate onSubmit;

        public void Initialize(string prompt, Actor actor, int maxOptions,
            SubmitSpellDelegate onSubmit)
        {
            promptText.text = prompt;
            this.maxOptions = maxOptions;
            this.onSubmit = onSubmit;

            for (int i = 0; i < actor.Spells.Count; i++)
            {
                GameObject optionObj = Instantiate(optionPrefab, listTransform);
                SpellModalListOption option = optionObj.GetComponent<SpellModalListOption>();
                option.Initialize(actor.Spells[i], SelectSpell);
            }
        }

        public void SelectSpell(SpellModalListOption option)
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
            LogModal("Spell modal list submitting...");
            onSubmit?.Invoke(selected[0].Spell);
            selected.Clear();
            Clean();
        }

        [System.Diagnostics.Conditional("DEBUG_MODAL")]
        public void LogModal(string logMsg)
        {
            UnityEngine.Debug.Log(logMsg);
        }
    }
}
