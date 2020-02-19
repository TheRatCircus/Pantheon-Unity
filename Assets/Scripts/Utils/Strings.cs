// Strings.cs
// Jerome Martina

using Pantheon.Components.Entity;
using System;
using System.Linq;

namespace Pantheon.Utils
{
    public static class Strings
    {
        // Courtesy of Carlos Muñoz
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

        public static string Possessive(Entity entity)
        {
            if (entity.Flags.HasFlag(EntityFlag.Male))
                return "his";
            else if (entity.Flags.HasFlag(EntityFlag.Female))
                return "her";
            else
                return "its";
        }

        public static string Subject(Entity entity)
        {
            if (entity.TryGetComponent(out Relic relic))
                return relic.Name;
            else if (entity.TryGetComponent(out Actor actor))
            {
                if (actor.Control == ActorControl.Player)
                    return "you";
                else if (entity.Unique)
                    return entity.Name;
                else
                    return $"the {entity.Name}";
            }
            else
                return $"the {entity.Name}";
        }

        public static string Subject(Entity entity, bool sentenceStart)
        {
            if (entity.TryGetComponent(out Relic relic))
                return relic.Name;
            else if (entity.TryGetComponent(out Actor actor))
            {
                if (actor.Control == ActorControl.Player)
                    return sentenceStart ? "You" : "you";
                else if (entity.Unique)
                    return entity.Name;
                else
                    return sentenceStart ? $"The {entity.Name}" : $"the {entity.Name}";
            }
            else
                return sentenceStart ? $"The {entity.Name}" : $"the {entity.Name}";
        }
    }
}
