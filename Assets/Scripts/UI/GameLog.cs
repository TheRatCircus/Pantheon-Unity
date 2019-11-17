// GameLog.cs
// Jerome Martina 

using UnityEngine;
using TMPro;

namespace Pantheon.UI
{
    public sealed class GameLog : MonoBehaviour
    {
        [SerializeField] private Transform logTransform = default;
        [SerializeField] private GameObject msgPrefab = default;

        public void Send(string msg, Color color)
        {
            GameObject msgObj = Instantiate(msgPrefab, logTransform);
            TextMeshProUGUI msgText = msgObj.GetComponent<TextMeshProUGUI>();
            msgText.text = msg;
            msgText.color = color;
        }
    }
}
