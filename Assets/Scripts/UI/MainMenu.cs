// MainMenu.cs
// Jerome Martina

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
        [SerializeField] private GameObject saveOptionPrefab = null;

        [SerializeField] private GameObject mainTitle = null;
        [SerializeField] private GameObject loadMenu = null;
        [SerializeField] private Transform saveOptionsList = null;

        public void NewGame()
        {
            SceneManager.LoadScene(Scenes.Intro, LoadSceneMode.Single);
        }

        //public void OpenLoadMenu()
        //{
        //    mainTitle.SetActive(false);
        //    loadMenu.SetActive(true);

        //    string[] saveFiles = Directory.GetFiles
        //        (Application.persistentDataPath, "*.save",
        //        SearchOption.AllDirectories);

        //    BinaryFormatter formatter = new BinaryFormatter();
        //    foreach (string filePath in saveFiles)
        //    {
        //        FileStream stream = new FileStream(filePath, FileMode.Open);
        //        Save save = formatter.Deserialize(stream) as Save;
        //        GameObject saveOption = Instantiate(saveOptionPrefab,
        //            saveOptionsList);
        //        Button saveOptionBtn = saveOption.GetComponent<Button>();
        //        Text saveOptionLabel
        //            = saveOption.GetComponentInChildren<Text>();
        //        saveOptionLabel.text = save.SaveName;

        //        saveOptionBtn.onClick.AddListener
        //            (delegate { StartCoroutine(LoadGame(save)); });
        //    }
        //}

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
