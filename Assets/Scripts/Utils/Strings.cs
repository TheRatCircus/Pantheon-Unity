// Strings.cs
// Jerome Martina

using Pantheon.Actors;

namespace Pantheon.Utils
{
    /// <summary>
    /// Functions for string processing.
    /// </summary>
    public static class Strings
    {
        /// <summary>
        /// Returns a subject string, with no trailing whitespace.
        /// </summary>
        /// <param name="actor">The subject at hand.</param>
        /// <param name="sentenceStart">Capitalize if true.</param>
        /// <returns></returns>
        public static string GetSubject(Actor actor, bool sentenceStart)
        {
            string ret = "";
            if (actor is Player)
                ret = sentenceStart ? "You" : "you";
            else
                ret = $"{(actor.NameIsProper ? "" : (sentenceStart ? "The " : "the "))}{actor.ActorName}";
            return ret;
        }

        public static string WeaponHitString(Actor actor, Item item)
        {
            string ret;
            bool isPlayer = actor is Player;

            switch (item.Melee.DamageType)
            {
                case DamageType.Slashing:
                    ret = SlashingVerbs.Random(false).Get(isPlayer);
                    break;
                case DamageType.Piercing:
                    ret = PiercingVerbs.Random(false).Get(isPlayer);
                    break;
                case DamageType.Bludgeoning:
                    ret = BludgeoningVerbs.Random(false).Get(isPlayer);
                    break;
                default:
                    throw new System.Exception
                        ($"Case missing for damage type {item.Melee.DamageType.ToString()}.");
            }

            return ret;
        }

        public static string PartHitString(Actor actor, BodyPart part)
        {
            string ret;
            bool isPlayer = actor is Player;

            switch (part.Type)
            {
                case BodyPartType.Arm:
                    ret = PunchVerbs.Random(false).Get(isPlayer);
                    break;
                case BodyPartType.Head:
                    ret = HeadButt.Get(isPlayer);
                    break;
                case BodyPartType.Teeth:
                    ret = BiteVerbs.Random(false).Get(isPlayer);
                    break;
                case BodyPartType.Claw:
                    ret = ClawVerbs.Random(false).Get(isPlayer);
                    break;
                default:
                    throw new System.Exception
                        ($"Case missing for {part.Type.ToString()}-based attack.");
            }

            return ret;
        }

        public static string[] RestMessages =
        {
            "to rest a while.",
            "to take a breather.",
            "to relax a moment."
        };

        public static Verb[] BiteVerbs =
        {
            new Verb("bite", "bites")
        };

        public static Verb[] ClawVerbs =
        {
            new Verb("claw", "claws"),
            new Verb("rend", "rends")
        };

        public static Verb[] PunchVerbs =
        {
            new Verb("punch", "punches"),
            new Verb("jab", "jabs")
        };

        public static Verb HeadButt = new Verb("headbutt", "headbutts");

        public static Verb[] SlashingVerbs = 
        {
            new Verb("slash", "slashes"),
            new Verb("slice", "slices"),
            new Verb("cut", "cuts")
        };

        public static Verb[] PiercingVerbs =
        {
            new Verb("pierce", "pierces"),
            new Verb("puncture", "punctures"),
            new Verb("perforate", "perforates")
        };

        public static Verb[] BludgeoningVerbs =
        {
            new Verb("bludgeon", "bludgeons"),
            new Verb("smash", "smashes"),
            new Verb("crush", "crushes")
        };

        /// <summary>
        /// Apply rich text colour to a string.
        /// </summary>
        /// <param name="str">String to be coloured.</param>
        /// <param name="colour">Rich text colour, enumerated.</param>
        /// <returns>str, coloured using colour.</returns>
        public static string ColourString(string str, TextColour colour)
        {
            // Unity's Color class can't be translated to rich text styling, so
            // an enum is used
            string colourStyleStr;
            switch (colour)
            {
                case TextColour.White:
                    colourStyleStr = "white";
                    break;
                case TextColour.Grey:
                    colourStyleStr = "grey";
                    break;
                case TextColour.Yellow:
                    colourStyleStr = "yellow";
                    break;
                case TextColour.Red:
                    colourStyleStr = "red";
                    break;
                case TextColour.Purple:
                    colourStyleStr = "purple";
                    break;
                case TextColour.Teal:
                    colourStyleStr = "teal";
                    break;
                case TextColour.Orange:
                    colourStyleStr = "orange";
                    break;
                case TextColour.Green:
                    colourStyleStr = "green";
                    break;
                case TextColour.Blue: // Actual blue is ugly on dark background
                    colourStyleStr = "lightblue"; 
                    break;
                default:
                    UnityEngine.Debug.LogWarning
                        ($"Colour {colour.ToString()} not found:" +
                        $" defaulting to white.");
                    colourStyleStr = "white";
                    break;
            }

            // Apply HTML colour styling to string
            string styledStr = $"<color={colourStyleStr}>{str}</color>";
            return styledStr;
        }

        // Colours with which to style rich text
        public enum TextColour
        {
            White,
            Grey,
            Yellow,
            Red,
            Purple,
            Teal,
            Orange,
            Green,
            Blue
        }
    }

    /// <summary>
    /// Holds a verb's 2nd and 3rd conjugations.
    /// </summary>
    public struct Verb
    {
        private readonly string second;
        private readonly string third;

        public Verb(string second, string third)
        {
            this.second = second;
            this.third = third;
        }

        public string Get(bool isPlayer) => isPlayer ? second : third;
    }
}