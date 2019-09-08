// Main menu handler
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pantheon.UI
{
    public class MainMenu : MonoBehaviour
    {
        public void NewGame()
        {
            SceneManager.LoadScene("Dungeon 1");
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
