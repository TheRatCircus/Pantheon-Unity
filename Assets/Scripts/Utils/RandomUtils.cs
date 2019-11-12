// RandomUtils.cs
// Jerome Martina

using System.Collections.Generic;

namespace Pantheon.Utils
{
    public static class RandomUtils
    {
        // List.Random() extension method
        public static T Random<T>(this List<T> list, bool seeded)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}
