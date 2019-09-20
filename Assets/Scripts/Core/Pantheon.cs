// Pantheon.cs
// Jerome Martina

using System.Collections.Generic;
using Pantheon.Actors;
using Pantheon.WorldGen;

namespace Pantheon.Core
{
    /// <summary>
    /// Global storage and control of Idol data.
    /// </summary>
    public class Pantheon
    {
        public const int Size = 4;

        public Dictionary<string, Idol> Idols { get; set; }
            = new Dictionary<string, Idol>();

        public Pantheon()
        {
            UnityEngine.Debug.Log("Populating the Pantheon...");
            for (int i = 0; i < Size; i++)
            {
                Idol idol = new Idol { DisplayName = CharacterNames.Random() };
                idol.RefName = idol.DisplayName.ToLower();

                if (Utils.RandomUtils.CoinFlip())
                    idol.Gender = Gender.Male;
                else
                    idol.Gender = Gender.Female;

                AssignAspect(idol);

                // TODO: Personality, mannerism, titles

                Idols.Add(idol.RefName, idol);
                UnityEngine.Debug.Log($"Finished idol {idol}.");
            }
        }

        public void AssignAspect(Idol idol)
        {
            for (int i = 0; i < 50; i++)
            {
                Aspect aspect = Database.RandomAspect();
                bool occupied = false;

                // Check if another Idol has this aspect
                foreach (KeyValuePair<string, Idol> pair in Idols)
                {
                    if (pair.Value.Aspect == aspect)
                    {
                        occupied = true;
                        break;
                    }
                }

                if (!occupied)
                {
                    idol.Aspect = aspect;
                    return;
                }
            }
            throw new System.Exception
                ("No open aspect found for Idol after 50 attempts.");
        }
    }
}
