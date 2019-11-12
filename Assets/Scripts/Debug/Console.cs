// Console.cs
// Jerome Martina

using Pantheon.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Pantheon.Debug.ConsoleCommandFunctions;

namespace Pantheon.Debug
{
    /// <summary>
    /// Takes commands for in-game debugging.
    /// </summary>
    public class Console : MonoBehaviour
    {
        [SerializeField] private GameController controller = default;
        [SerializeField] private GameObject console = default;
        [SerializeField] private Text consoleLog = default;
        [SerializeField] private InputField input = default;

        private List<string> logEntries = new List<string>();

        private Dictionary<string, ConsoleCommand> consoleCommands =
            new Dictionary<string, ConsoleCommand>()
            {
                { "list_layers", new ConsoleCommand(ListLayers) },
                { "list_levels", new ConsoleCommand(ListLevels) },
                { "where_am_i", new ConsoleCommand(WhereAmI) },
                { "describe_cell", new ConsoleCommand(DescribeCell) },
                { "find_entities", new ConsoleCommand(FindEntities) },
                { "describe", new ConsoleCommand(DescribeEntity) }
            };

        private void Awake()
        {
            Scene game = SceneManager.GetSceneByName(Utils.Scenes.Game);
            foreach (GameObject go in game.GetRootGameObjects())
            {
                if (go.tag == "GameController")
                {
                    controller = go.GetComponent<GameController>();
                    break;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Console"))
            {
                if (console.activeInHierarchy)
                {
                    input.text = "";
                    console.SetActive(false);
                    controller.AllowInputToCharacter(false);
                }
                else
                {
                    console.SetActive(true);
                    input.Select();
                    input.ActivateInputField();
                    controller.AllowInputToCharacter(false);
                }
            }

            if (Input.GetButtonDown("Submit"))
            {
                if (input.text != "")
                    SubmitCommand(input.text);
            }
        }

        void SubmitCommand(string input)
        {
            this.input.text = "";
            string[] tokens = input.Split(' ');
            string output;

            if (!consoleCommands.TryGetValue(tokens[0],
                out ConsoleCommand cmd))
                output = $"Command \"{tokens[0]}\" not found";
            else
            {
                output = tokens[0];

                string[] args = new string[tokens.Length - 1];
                Array.Copy(tokens, 1, args, 0, tokens.Length - 1);

                output = cmd.action.Invoke(args, controller);
            }

            logEntries.Add(output);

            string newLog = null;

            foreach (string s in logEntries)
                newLog += $"{s}{Environment.NewLine}";

            consoleLog.text = newLog;
            this.input.Select();
            this.input.ActivateInputField();
        }
    }

    public class ConsoleCommand
    {
        public Func<string[], GameController, string> action;

        public ConsoleCommand(Func<string[], GameController, string> action)
            => this.action = action;
    }
}
