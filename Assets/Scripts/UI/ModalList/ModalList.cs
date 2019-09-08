// ModalList.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.UI
{
    public class ModalList : MonoBehaviour
    {
        [SerializeField] protected GameObject optionPrefab;

        // UI elements
        [SerializeField] protected Text promptText;
        [SerializeField] protected Transform listTransform;

        // Parameters
        protected int maxOptions;

        // Clean this modal list of all options
        public void Clean()
        {
            foreach (Transform t in listTransform)
                Destroy(t.gameObject);
        }
    }

}
