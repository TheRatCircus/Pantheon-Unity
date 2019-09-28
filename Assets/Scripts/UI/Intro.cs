// Intro.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum IntroState
{
    IntroText,
    NameInput,
    Background
}

namespace Pantheon
{
    /// <summary>
    /// Controller for intro screens.
    /// </summary>
    public class Intro : MonoBehaviour
    {
        public IntroState IntroState { get; set; }
            = IntroState.IntroText;

        [SerializeField] private GameObject introText = null;

        [SerializeField] private GameObject nameInput = null;
        [SerializeField] private InputField inputField = null;
        [SerializeField] private Text nameSelectPrompt = null;

        [SerializeField] private GameObject backgroundSelect = null;

        public string PlayerName { get; private set; }
        public WeaponType StartingWeapon { get; private set; }

#if UNITY_EDITOR
        private void Start()
        {
            PlayerName = "The Hero";
            StartingWeapon = WeaponType.Hatchet;
            StartGame();
        }
#endif

        // Update is called once per frame
        private void Update()
        {
            if (Input.anyKeyDown && IntroState == IntroState.IntroText)
            {
                introText.SetActive(false);
                nameInput.SetActive(true);
                IntroState = IntroState.NameInput;
            }
        }

        public void ConfirmName()
        {
            if (inputField.text != "")
            {
                PlayerName = inputField.text;
                nameInput.SetActive(false);
                backgroundSelect.SetActive(true);
                IntroState = IntroState.Background;
            }
            else
            {
                nameSelectPrompt.text
                    = "But they will have to call you something" +
                    " when they etch your name into legend!";
            }
        }

        public void SelectBackground(int bg)
        {
            switch (bg)
            {
                case 0: // Hatchet
                    StartingWeapon = WeaponType.Hatchet;
                    break;
            }
            backgroundSelect.SetActive(false);
            StartGame();
        }

        private void StartGame()
        {
            SceneManager.LoadScene("Debug", LoadSceneMode.Additive);

            GameObject[] rootGameObjectsOfSpecificScene
                = SceneManager.GetSceneByName("Main")
                .GetRootGameObjects();

            foreach (GameObject go in rootGameObjectsOfSpecificScene)
                go.SetActive(true);
        }
    }
}
