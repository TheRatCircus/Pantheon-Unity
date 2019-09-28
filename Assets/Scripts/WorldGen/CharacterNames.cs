// CharacterNames.cs
// Jerome Martina

using Pantheon.Utils;

namespace Pantheon.WorldGen
{
    public static class CharacterNames
    {
        public class CharacterName
        {
            public readonly string Name = null;
            public bool Used = false;

            public CharacterName(string name)
                => Name = name;
        }

        public static CharacterName[] _characterNames =
        {
            new CharacterName("Almy"),
            new CharacterName("Aronow"),
            new CharacterName("Brume"),
            new CharacterName("Haggerty"),
            new CharacterName("Ignacy"),
            new CharacterName("Larn"),
            new CharacterName("Kneller"),
            new CharacterName("Waratah")
        };

        public static string Random()
        {
            CharacterName ret;
            int attempts = 0;

            do
            {
                if (attempts > 100)
                    throw new System.Exception
                        ("Could not find a random character name.");

                ret = _characterNames.Random(true);
                attempts++;

            } while (ret.Used);

            ret.Used = true;
            return ret.Name;
        }

        public static void ClearUsed()
        {
            foreach (CharacterName name in _characterNames)
                name.Used = false;
        }
    }
}
