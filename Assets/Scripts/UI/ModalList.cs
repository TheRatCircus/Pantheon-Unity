// ModalList.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.UI
{
    public sealed class ModalList : MonoBehaviour
    {
        [SerializeField] private GameObject optionPrefab = default;

        [SerializeField] private TextMeshProUGUI prompt = default;
        [SerializeField] private Transform contentTransform = default;
        private List<GameObject> options = new List<GameObject>();

        public void SetPrompt(string prompt)
        {
            this.prompt.text = prompt;
        }

        public void Populate(int numberOfOptions)
        {
            for (int i = 0; i < numberOfOptions; i++)
            {
                GameObject optionObj = Instantiate(optionPrefab, contentTransform);
                options.Add(optionObj);
            }
        }

        public void SetOptionImage(int optionNo, Sprite sprite)
        {
            Image img = options[optionNo].transform.Find(
                "ModalListOptionImage").GetComponent<Image>();
            img.sprite = sprite;
        }

        public void SetOptionText(int optionNo, string text)
        {
            TextMeshProUGUI tm = options[optionNo].transform.Find(
                "ModalListOptionText").GetComponent<TextMeshProUGUI>();
            tm.text = text;
        }

        public void SetOptionCallback(int optionNo, Action<int> callback)
        {
            Button btn = options[optionNo].GetComponent<Button>();
            btn.onClick.AddListener(delegate 
            {
                callback.Invoke(optionNo);
                Destroy(gameObject);
            });
        }
    }
}
