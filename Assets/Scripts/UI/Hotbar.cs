// Hotbar.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.UI
{
    public sealed class Hotbar : MonoBehaviour
    {
        [SerializeField] private Button[] buttons = new Button[10];

        private void Start() => buttons[0].Select();

        public void SetSelected(int index) => buttons[index].Select();
    }
}
