// Console.cs
// Jerome Martina

using Pantheon.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using static Pantheon.Debug.ConsoleCommandFunctions;

namespace Pantheon.Debug
{
    /// <summary>
    /// Takes commands for in-game debugging.
    /// </summary>
    public sealed class Console : MonoBehaviour
    {
        [SerializeField] private GuidReference ctrlRef = default;
        [SerializeField] private GameController controller = default;
        [SerializeField] private GameObject console = default;
        [SerializeField] private Text consoleLog = default;
        [SerializeField] private InputField input = default;
        [SerializeField] private DebugInfo debugInfo = default;

        private List<string> logEntries = new List<string>();

        private Dictionary<string, ConsoleCommand> consoleCommands =
            new Dictionary<string, ConsoleCommand>()
            {
                { "list_layers", new ConsoleCommand(ListLayers) },
                { "list_levels", new ConsoleCommand(ListLevels) },
                { "loaded_assets", new ConsoleCommand(LoadedAssets) },
                { "reveal_level", new ConsoleCommand(RevealLevel) },
                { "spawn", new ConsoleCommand(Spawn) },
                { "turn_order", new ConsoleCommand(TurnOrder) },
                { "describe_component", new ConsoleCommand(DescribeComponent) },
                { "destroy", new ConsoleCommand(ConsoleCommandFunctions.Destroy) },
                { "idolmode", new ConsoleCommand(ToggleIdolMode) },
                { "give", new ConsoleCommand(Give) },
                { "teleport", new ConsoleCommand(Teleport) },
                { "strategy", new ConsoleCommand(Strategy) },
                { "relic", new ConsoleCommand(Relic) },
                { "enthrall", new ConsoleCommand(Enthrall) },
                { "vault", new ConsoleCommand(Vault) },
                { "travel", new ConsoleCommand(Travel) },
                { "kill_level", new ConsoleCommand(KillLevel) }
            };

        private void Awake()
        {
            Profiler.BeginSample("Console.Awake()");
            GameObject ctrlObj = ctrlRef.gameObject;
            controller = ctrlObj.GetComponent<GameController>();
            Profiler.EndSample();

            debugInfo.Initialize(controller);
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
                    controller.AllowInputToCharacter(true);
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
                if (input.text != "")
                    SubmitCommand(input.text);
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
