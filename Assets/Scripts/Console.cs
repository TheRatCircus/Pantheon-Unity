// Console.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pantheon.Core;
using Pantheon.World;

/// <summary>
/// Takes commands for in-game debugging.
/// </summary>
public class Console : MonoBehaviour
{
    [SerializeField] private GameObject console = null;
    [SerializeField] private Text consoleLog = null;
    [SerializeField] private InputField input = null;

    private List<string> logEntries = new List<string>();

    Dictionary<string, ConsoleCommand> consoleCommands;

    private void Awake()
    {
        consoleCommands = new Dictionary<string, ConsoleCommand>()
        {
            { "reveal_level", new ConsoleCommand(RevealLevel) }
        };
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
                Game.GetPlayer().Input.SetInputState(InputState.Move);
            }
            else
            {
                console.SetActive(true);
                Game.GetPlayer().Input.SetInputState(InputState.Console);
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

        if (!consoleCommands.TryGetValue(tokens[0], out ConsoleCommand cmd))
            output = $"Command \"{tokens[0]}\" not found";
        else
        {
            output = tokens[0];

            string[] args = new string[tokens.Length - 1];
            Array.Copy(tokens, 1, args, 0, tokens.Length - 1);

            output = cmd.action?.Invoke(args);
        }

        logEntries.Add(output);

        string newLog = null;

        foreach (string s in logEntries)
            newLog += $"{s}{Environment.NewLine}";

        consoleLog.text = newLog;
    }

    string RevealLevel(string[] args)
    {
        foreach (Cell cell in Game.instance.activeLevel.Map)
            cell.Reveal();

        Game.instance.activeLevel.RefreshFOV();

        return "Revealing level...";
    }
}

public class ConsoleCommand
{
    public Func<string[], string> action;

    public ConsoleCommand(Func<string[], string> action)
        => this.action = action;
}
