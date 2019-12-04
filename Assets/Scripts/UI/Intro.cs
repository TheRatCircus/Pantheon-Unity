// Intro.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.Utils;
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
    public sealed class Intro : MonoBehaviour
    {
        public IntroState IntroState { get; set; }
            = IntroState.IntroText;

        [SerializeField] private AudioListener audioListener = null;

        [SerializeField] private GameObject introText = null;

        [SerializeField] private GameObject nameInput = null;
        [SerializeField] private InputField inputField = null;
        [SerializeField] private Text nameSelectPrompt = null;

        public string PlayerName { get; private set; }

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
                audioListener.enabled = false;
                SceneManager.LoadSceneAsync(Scenes.Game, LoadSceneMode.Additive).
                completed += (AsyncOperation op) =>
                {
                    Scene gameScene = SceneManager.GetSceneByName(Scenes.Game);
                    SceneManager.SetActiveScene(gameScene);
                    SceneManager.LoadSceneAsync(Scenes.Debug, LoadSceneMode.Additive);
                    GameController.NewGame(PlayerName);
                    SceneManager.UnloadSceneAsync(Scenes.Intro);
                };
            }
            else
            {
                nameSelectPrompt.text
                    = "But they will have to call you something" +
                    " when they etch your name into legend!";
            }
        }
    }
}
