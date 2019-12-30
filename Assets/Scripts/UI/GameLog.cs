// GameLog.cs
// Jerome Martina 

using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Pantheon.UI
{
    public sealed class GameLog : MonoBehaviour, IGameLog
    {
        [SerializeField] private GameObject msgPrefab = default;

        [SerializeField] private Transform logTransform = default;
        [SerializeField] private ScrollRect scrollRect = default;

        public void Send(string msg, Color color)
        {
            GameObject msgObj = Instantiate(msgPrefab, logTransform);
            TextMeshProUGUI msgText = msgObj.GetComponent<TextMeshProUGUI>();
            msgText.text = msg;
            msgText.color = color;
            StartCoroutine(ScrollToBottom());
        }

        private System.Collections.IEnumerator ScrollToBottom()
        {
            yield return null;
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
