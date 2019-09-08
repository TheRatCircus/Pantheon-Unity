// ModalList.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.UI;

namespace Pantheon.UI
{
    public class ModalList : MonoBehaviour
    {
        public GameObject optionPrefab;

        // UI elements
        public Text promptText;
        public Transform listTransform;

        // Parameters
        public int maxOptions;

        // Clean this modal list of all options
        public void Clean()
        {
            foreach (Transform t in listTransform)
                Destroy(t.gameObject);
        }
    }

}
