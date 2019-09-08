// Handler for game event log

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pantheon.Actors;
using Pantheon.World;

// Colours with which to style rich text
public enum MessageColour
{
    White,
    Grey,
    Yellow,
    Red,
    Purple,
    Teal,
    Orange
}

namespace Pantheon.Core
{
    public class GameLog : MonoBehaviour
    {
        // The extended event list for the session
        [ReadOnly] private List<string> eventList = new List<string>();
        // Short list of event strings for HUD log
        [ReadOnly] private List<string> shortEventList = new List<string>();

        [SerializeField] private Text logText = null;

        public static GameLog GetLog() => Game.instance.GameLog;

        // Append the eventList with a new message and add it to the log
        public static void Send(string msg, MessageColour colour)
        {
            // Unity's Color class can't be translated to rich text styling, so
            // an enum is used
            string colourStyleStr;
            switch (colour)
            {
                case MessageColour.White:
                    colourStyleStr = "white";
                    break;
                case MessageColour.Grey:
                    colourStyleStr = "grey";
                    break;
                case MessageColour.Yellow:
                    colourStyleStr = "yellow";
                    break;
                case MessageColour.Red:
                    colourStyleStr = "red";
                    break;
                case MessageColour.Purple:
                    colourStyleStr = "purple";
                    break;
                case MessageColour.Teal:
                    colourStyleStr = "teal";
                    break;
                case MessageColour.Orange:
                    colourStyleStr = "orange";
                    break;
                default:
                    colourStyleStr = "white";
                    break;
            }

            // Apply HTML colour styling to string
            string styledStr = $"<color={colourStyleStr}>{msg}</color>";

            // Add event to both lists
            GetLog().eventList.Add(styledStr);
            GetLog().shortEventList.Add(styledStr);

            // Cull old messages from top of HUD log
            if (GetLog().logText.cachedTextGenerator.lines.Count >= 9)
                GetLog().shortEventList.RemoveAt(0);

            string logStr = "";
            foreach (string s in GetLog().shortEventList)
                logStr += $"{s}{Environment.NewLine}";

            GetLog().logText.text = logStr;
        }

        // Return a subject string
        public static string GetSubject(Actor actor, bool sentenceStart)
        {
            string ret = "";
            if (actor is Player)
                ret = sentenceStart ? "You" : "you";
            else
                ret = $"{(actor.NameIsProper ? "" : (sentenceStart ? "The " : "the "))}{actor.ActorName}";
            return ret;
        }

        // Compute a melee attack's game log entry
        public static string GetAttackString(Actor attacker, Actor defender, Hit hit)
        {
            string ret;

            string attackerSubject = GetSubject(attacker, true);
            string defenderSubject = GetSubject(defender, false);

            if (hit == null) // Miss
                ret = $"{attackerSubject} {(attacker is Player ? "miss" : "misses")} {defenderSubject}.";
            else
                ret = $"{attackerSubject} {(attacker is Player ? "hit" : "hits")} {defenderSubject} for {hit.Damage} damage!";

            return ret;
        }

        // Send the items in a cell to the game log
        public static void LogCellItems(Cell cell)
        {
            if (cell.Items.Count > 0)
            {
                string msg = $"You see here";
                int i = 0;
                for (; i < cell.Items.Count; i++)
                    msg += $" a {cell.Items[i].DisplayName};";
                GameLog.Send(msg, MessageColour.Grey);
            }
        }
    }
}
