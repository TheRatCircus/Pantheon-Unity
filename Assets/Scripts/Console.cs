// Console.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pantheon.Core;
using Pantheon.Actors;
using Pantheon.World;
using Pantheon.Utils;

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
            { "reveal_level", new ConsoleCommand(RevealLevel) },
            { "apply_status", new ConsoleCommand(ApplyStatus) },
            { "give_item", new ConsoleCommand(GiveItem) },
            { "learn_spell", new ConsoleCommand(LearnSpell) },
            { "add_trait", new ConsoleCommand(AddTrait) }
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
                input.Select();
                input.ActivateInputField();
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

    string ApplyStatus(string[] args)
    {
        if (args.Length != 1)
            return "Please only pass 1 argument.";

        StatusType statusType
            = (StatusType)Enum.Parse(typeof(StatusType), args[0]);

        StatusEffect status = StatusFactory.GetStatus(statusType);

        Game.GetPlayer().ApplyStatus(status);

        return $"Status effect {status.DisplayName} applied to player.";
    }

    string GiveItem(string[] args)
    {
        if (args.Length != 1)
            return "Please pass only 1 argument.";

        if (Enum.TryParse(args[0], out WeaponType weaponType))
        {
            Game.GetPlayer().Inventory.Add(ItemFactory.NewWeapon(weaponType));
            Game.GetPlayer().RaiseInventoryChangeEvent();
            return $"Giving {weaponType.ToString()}";
        }
        else if (Enum.TryParse(args[0], out FlaskType flaskType))
        {
            Game.GetPlayer().Inventory.Add(ItemFactory.NewFlask(flaskType));
            Game.GetPlayer().RaiseInventoryChangeEvent();
            return $"Giving {flaskType.ToString()}";
        }
        else if (Enum.TryParse(args[0], out ScrollType scrollType))
        {
            Game.GetPlayer().Inventory.Add(ItemFactory.NewScroll(scrollType));
            Game.GetPlayer().RaiseInventoryChangeEvent();
            return $"Giving {scrollType.ToString()}";
        }
        else
            return $"Item of type {args[0]} could not be found";
    }

    string LearnSpell(string[] args)
    {
        if (args.Length != 1)
            return "Please pass only 1 argument.";

        if (Enum.TryParse(args[0], out SpellType spellType))
        {
            Game.GetPlayer().Spells.Add(Database.GetSpell(spellType));
            return $"You have learned {spellType.ToString()}";
        }
        else
            return $"Spell of type {args[0]} could not be found";
    }

    string AddTrait(string[] args)
    {
        if (args.Length != 1)
            return "Please pass only 1 argument.";

        if (Traits.traitsLookup.TryGetValue(args[0], out Trait trait))
        {
            Game.GetPlayer().AddTrait(trait);
            return $"Added trait {trait}";
        }
        else
            return $"Could not find trait {args[0]}";
    }
}

public class ConsoleCommand
{
    public Func<string[], string> action;

    public ConsoleCommand(Func<string[], string> action)
        => this.action = action;
}
