// GameLog.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pantheon.Utils;
using Pantheon.World;

namespace Pantheon.Core
{
    /// <summary>
    /// Handler for in-game event log.
    /// </summary>
    public class GameLog : MonoBehaviour
    {
        // The extended event list for the session
        [SerializeField]
        [ReadOnly]
        private List<string> eventList = new List<string>();
        // Short list of event strings for HUD log
        [SerializeField]
        [ReadOnly]
        private List<string> shortEventList = new List<string>();

        [SerializeField] private Text logText = null;

        public static GameLog GetLog() => Game.instance.GameLog;

        // Append the eventList with a new message and add it to the log
        public static void Send(string msg, Strings.TextColour colour)
        {
            msg = Strings.ColourString(msg, colour);

            // Add event to both lists
            GetLog().eventList.Add(msg);
            GetLog().shortEventList.Add(msg);

            // Cull old messages from top of HUD log
            if (GetLog().logText.cachedTextGenerator.lines.Count >= 8)
                GetLog().shortEventList.RemoveAt(0);

            string logStr = "";
            foreach (string s in GetLog().shortEventList)
                logStr += $"{s}{Environment.NewLine}";

            GetLog().logText.text = logStr;
        }

        public static void Send(string msg)
        {
            // Add event to both lists
            GetLog().eventList.Add(msg);
            GetLog().shortEventList.Add(msg);

            // Cull old messages from top of HUD log
            if (GetLog().logText.cachedTextGenerator.lines.Count >= 8)
                GetLog().shortEventList.RemoveAt(0);

            string logStr = "";
            foreach (string s in GetLog().shortEventList)
                logStr += $"{s}{Environment.NewLine}";

            GetLog().logText.text = logStr;
        }

        public static void LogCellItems(Cell cell)
        {
            string msg = $"You see here";

            if (cell.Items.Count == 1)
                msg += $" a {cell.Items[0].DisplayName}.";
            else
            {
                int i = 0;
                for (; i < cell.Items.Count - 1; i++)
                    msg += $" a {cell.Items[i].DisplayName},";
                msg += $" and a {cell.Items[i].DisplayName}.";
            }

            Send(msg, Strings.TextColour.Grey);
        }

        public static void LogCellFeature(Cell cell)
        {
            string msg = $"There is {cell.Feature.DisplayName} here.";
            Send(msg, Strings.TextColour.Grey);
        }

        public static void LogCellConnection(Cell cell)
        {
            string msg = $"There is {cell.Connection.DisplayName} here.";
            Send(msg, Strings.TextColour.Grey);
        }
    }
}
