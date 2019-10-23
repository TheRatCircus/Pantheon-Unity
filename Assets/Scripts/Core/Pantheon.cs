// Pantheon.cs
// Jerome Martina

using System.Collections.Generic;
using Pantheon.Utils;

namespace Pantheon.Core
{
    /// <summary>
    /// Global storage and control of Idol data.
    /// </summary>
    [System.Serializable]
    public sealed class Pantheon
    {
        public const int Size = 4;

        public Dictionary<string, Idol> Idols { get; set; }
            = new Dictionary<string, Idol>();

        public Pantheon()
        {
            UnityEngine.Debug.Log("Building pantheon...");
            for (int i = 0; i < Size; i++)
            {
                Idol idol = new Idol();
                Markov m = new Markov();
                idol.DisplayName = m.GetName();
                idol.RefName = idol.DisplayName.ToLower();

                if (RandomUtils.CoinFlip(true))
                    idol.Gender = Gender.Male;
                else
                    idol.Gender = Gender.Female;

                // TODO: Personality, mannerism, titles

                Idols.Add(idol.RefName, idol);
            }
            //AssignAspects();
        }

        public void AssignAspects()
        {
            Dictionary<AspectID, Aspect> aspects =
                Game.instance.Database.AspectDict;

            List<Aspect> shuffledAspects = new List<Aspect>(
                aspects.Count);

            foreach (Aspect a in aspects.Values)
                shuffledAspects.Add(a);

            shuffledAspects.Shuffle(true);

            int counter = 0;
            foreach (Idol idol in Idols.Values)
            {
                idol.Aspect = shuffledAspects[counter];
                counter++;
            }
        }
    }
}
