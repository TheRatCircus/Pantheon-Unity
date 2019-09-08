// ModalListOption.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.UI;
using Pantheon.Utils;

namespace Pantheon.UI
{
    /// <summary>
    /// Base class for all modal list options.
    /// </summary>
    public class ModalListOption : MonoBehaviour
    {
        [SerializeField] protected Image background;
        [SerializeField] protected Image icon;
        [SerializeField] protected Text text;

        // Highlight this option as selected
        public void SetSelected(bool selected)
        {
            background.color =
                (selected ?
                background.color = Colours.UISelected :
                background.color = Colours.UIBackground);
        }
    }

}
