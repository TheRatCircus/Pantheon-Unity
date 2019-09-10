// MainMenu.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pantheon.UI
{
    public class MainMenu : MonoBehaviour
    {
        // Skip main menu if in editor
        private void Awake()
        {
#if UNITY_EDITOR
            SceneManager.LoadScene("Main");
            SceneManager.LoadScene("Debug", LoadSceneMode.Additive);
#endif
        }

        public void NewGame() => SceneManager.LoadScene("Main");

        public void Quit() =>
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
