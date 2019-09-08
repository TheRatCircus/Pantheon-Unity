// ModalList.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.UI
{
    /// <summary>
    /// A list of selectable options which pops up for the player.
    /// </summary>
    public class ModalList : MonoBehaviour
    {
        [SerializeField] protected GameObject optionPrefab;

        // UI elements
        [SerializeField] protected Text promptText;
        [SerializeField] protected Transform listTransform;

        // Parameters
        protected int maxOptions;

        /// <summary>
        /// Clean this list of all options.
        /// </summary>
        public void Clean()
        {
            foreach (Transform t in listTransform)
                Destroy(t.gameObject);
        }
    }

}
