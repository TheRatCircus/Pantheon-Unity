// MainMenu.cs
// Jerome Martina

using Pantheon.Core;
using Pantheon.SaveLoad;
using Pantheon.Utils;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pantheon.UI
{
    public sealed class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject saveOptionPrefab = default;

        [SerializeField] private GuidReference ctrlRef = default;
        [SerializeField] private GameObject mainTitle = default;
        [SerializeField] private GameObject loadMenu = default;
        [SerializeField] private Transform saveOptionsList = default;

#if UNITY_EDITOR
        private void Awake() => NewGame();
#endif

        public void NewGame()
        {
            SceneManager.LoadScene(Scenes.Intro, LoadSceneMode.Single);
        }

        public void OpenLoadMenu()
        {
            mainTitle.SetActive(false);
            loadMenu.SetActive(true);

            string[] saveFiles = Directory.GetFiles
                (Application.persistentDataPath, "*.save",
                SearchOption.AllDirectories);

            BinaryFormatter formatter = new BinaryFormatter();

            foreach (string filePath in saveFiles)
            {
                GameObject saveOption = Instantiate(saveOptionPrefab,
                    saveOptionsList);
                Button saveOptionBtn = saveOption.GetComponent<Button>();
                Text saveOptionLabel
                    = saveOption.GetComponentInChildren<Text>();
                saveOptionLabel.text = Save.ReadSaveName(filePath);

                saveOptionBtn.onClick.AddListener
                    (delegate {
                        LoadGame(filePath);
                    });
            }
        }

        public void LoadGame(string path)
        {
            SceneManager.LoadSceneAsync(Scenes.Game, LoadSceneMode.Additive).
                completed += (AsyncOperation op) =>
                {
                    Scene gameScene = SceneManager.GetSceneByName(Scenes.Game);
                    SceneManager.SetActiveScene(gameScene);
                    SceneManager.LoadSceneAsync(Scenes.Debug, LoadSceneMode.Additive);
                    ctrlRef.gameObject.GetComponent<GameController>().LoadGame(path);
                    SceneManager.UnloadSceneAsync(Scenes.MainMenu);
                };
        }

        public void ToTitle()
        {
            loadMenu.SetActive(false);
            mainTitle.SetActive(true);
        }

        //public System.Collections.IEnumerator LoadGame(Save save)
        //{
        //    AsyncOperation load = SceneManager.LoadSceneAsync(Scenes.Game);

        //    while (!load.isDone)
        //        yield return null;

        //    SceneManager.LoadSceneAsync(Scenes.Game, LoadSceneMode.Additive).
        //        completed += (AsyncOperation op) =>
        //        {
        //            SceneManager.UnloadSceneAsync(gameObject.scene);
        //        };
        //}

        public void Quit() =>
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
