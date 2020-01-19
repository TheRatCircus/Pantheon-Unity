// Hotbar.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.UI
{
    public sealed class Hotbar : MonoBehaviour
    {
        [SerializeField] private Button[] buttons = new Button[10];
        [SerializeField] private Image[] icons = new Image[10];

        private void Start() => buttons[0].Select();

        public void SetSelected(int index) => buttons[index].Select();

        public void SetSprite(int index, Sprite sprite)
        {
            if (sprite != null)
                icons[index].gameObject.SetActive(true);
            else
                icons[index].gameObject.SetActive(false);

            icons[index].sprite = sprite;
        }
    }
}
