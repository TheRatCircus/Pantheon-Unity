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
    Purple,
    Teal
}

public class GameLog : MonoBehaviour
{
    // Singleton
    public static GameLog instance;

    // The extended event list for the session
    public List<string> eventList = new List<string>();
    // Short list of event strings for HUD log
    public List<string> shortEventList = new List<string>();

    public Text logText;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Database singleton assigned in error");
        else
            instance = this;
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
            default:
                colourStyleStr = "white";
                break;
        }

        // Apply HTML colour styling to string
        string styledStr = $"<color={colourStyleStr}>{msg}</color>";

        // Add event to both lists
        instance.eventList.Add(styledStr);
        instance.shortEventList.Add(styledStr);

        // Cull old messages from top of HUD log
        if (instance.logText.cachedTextGenerator.lines.Count >= 9)
            instance.shortEventList.RemoveAt(0);
            
        string logStr = "";
        foreach (string s in instance.shortEventList)
            logStr += $"{s}{Environment.NewLine}";

        instance.logText.text = logStr;
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
}
