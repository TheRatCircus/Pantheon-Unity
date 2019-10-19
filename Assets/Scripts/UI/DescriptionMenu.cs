// DescriptionMenu.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.UI
{
    public class DescriptionMenu : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Text titleText;
        [SerializeField] private Text descriptionText;

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void DescribeItem(Item item)
        {
            image.sprite = item.Sprite;
            titleText.text = item.DisplayName;

            descriptionText.text = "";

            foreach (Enchant e in item.Enchants)
            {
                descriptionText.text += $"{e}{System.Environment.NewLine}";
            }
        }
    }
}
