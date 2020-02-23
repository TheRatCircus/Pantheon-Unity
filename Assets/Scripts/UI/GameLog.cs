// GameLog.cs
// Jerome Martina 

using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.UI
{
    public sealed class GameLog : MonoBehaviour, IGameLog
    {
        [SerializeField] private GameObject msgPrefab = default;

        [SerializeField] private Transform logTransform = default;
        [SerializeField] private ScrollRect scrollRect = default;

        // TODO: Static Send()

        public void Send(string msg, Color color)
        {
            GameObject msgObj = Instantiate(msgPrefab, logTransform);
            Text msgText = msgObj.GetComponent<Text>();
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
