// Strings.cs
// Jerome Martina

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
    }
}
