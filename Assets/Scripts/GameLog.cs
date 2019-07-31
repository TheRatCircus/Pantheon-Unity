// Handler for game event log
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Colours with which to style rich text
public enum MessageColour
{
    White,
    Grey,
    Yellow,
    Red,
    Purple
}

public class GameLog : MonoBehaviour
{
    // Singleton
    public static GameLog gameLog;

    // The extended event list for the session
    public List<string> eventList = new List<string>();
    // Short list of event strings for HUD log
    public List<string> shortEventList = new List<string>();

    public Text logText;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        gameLog = this;
    }

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
            case MessageColour.Yellow:
                colourStyleStr = "yellow";
                break;
            case MessageColour.Red:
                colourStyleStr = "red";
                break;
            default:
                colourStyleStr = "white";
                break;
        }

        // Apply HTML colour styling to string
        string styledStr = $"<color={colourStyleStr}>{msg}</color>";

        // Add event to both lists
        gameLog.eventList.Add(styledStr);
        gameLog.shortEventList.Add(styledStr);

        // Cull old messages from top of HUD log
        if (gameLog.logText.cachedTextGenerator.lines.Count >= 6)
            gameLog.shortEventList.RemoveAt(0);
            
        string logStr = "";
        foreach (string s in gameLog.shortEventList)
            logStr += $"{s}{Environment.NewLine}";

        gameLog.logText.text = logStr;
    }
}
