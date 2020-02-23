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
        [SerializeField] private RectTransform selection = default;

        private void Start()
        {
            selection.SetParent(buttons[0].transform, false);
        }

        public void SetSelected(int index)
        {
            selection.SetParent(buttons[index].transform, false);
            RectTransform transform = (RectTransform)buttons[index].transform;
            Vector3 pos = transform.position;
            pos.y -= transform.rect.height / 3;
            selection.SetPositionAndRotation(pos, Quaternion.identity);
        }

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
