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
                    ret = RandomUtils.ArrayRandom(SlashingVerbs).Get(isPlayer);
                    break;
                case DamageType.Piercing:
                    ret = RandomUtils.ArrayRandom(PiercingVerbs).Get(isPlayer);
                    break;
                case DamageType.Bludgeoning:
                    ret = RandomUtils.ArrayRandom(BludgeoningVerbs).Get(isPlayer);
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
                    ret = RandomUtils.ArrayRandom(PunchVerbs).Get(isPlayer);
                    break;
                case BodyPartType.Head:
                    ret = HeadButt.Get(isPlayer);
                    break;
                default:
                    throw new System.Exception
                        ($"Case missing for {part.Type.ToString()}-based attack.");
            }

            return ret;
        }

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
        /// <param name="colour">Possible rich text colour, enumerated.</param>
        /// <returns>str, coloured using colour.</returns>
        public static string ColourString(string str, MessageColour colour)
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
            string styledStr = $"<color={colourStyleStr}>{str}</color>";
            return styledStr;
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