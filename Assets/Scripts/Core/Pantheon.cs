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
                string idolName = CharacterNames.Random();
                string refName = idolName.ToLower();
                Idols.Add(refName, new Idol(idolName, refName));
            }
        }
    }
}
