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

        [SerializeField] private GameObject backgroundSelect = null;

        public string PlayerName { get; private set; }

#if UNITY_EDITOR
        private void Start()
        {
            //GameObject[] rootGameObjectsOfSpecificScene
            //    = SceneManager.GetSceneByName(Scenes.Main)
            //    .GetRootGameObjects();

            //foreach (GameObject go in rootGameObjectsOfSpecificScene)
            //{
            //    if (!go == gameObject && !go.CompareTag("EventSystem"))
            //    {
            //        go.SetActive(false);
            //    }
            //}

            //PlayerName = "The Hero";
            //StartingWeapon = WeaponType.Hatchet;
            //StartGame();
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

        public void SelectBackground()
        {
            backgroundSelect.SetActive(false);
            audioListener.enabled = false;

            
            SceneManager.LoadSceneAsync(Scenes.Game, LoadSceneMode.Additive).
                completed += (AsyncOperation op) =>
                {
                    //Core.Game.instance.NewGame(PlayerName, StartingWeapon);
                    Scene gameScene = SceneManager.GetSceneByName(Scenes.Game);
                    SceneManager.SetActiveScene(gameScene);
                    SceneManager.LoadSceneAsync(Scenes.Debug, LoadSceneMode.Additive);
                    GameController.NewGame(PlayerName);
                    SceneManager.UnloadSceneAsync(Scenes.Intro);
                };
        }
    }
}
