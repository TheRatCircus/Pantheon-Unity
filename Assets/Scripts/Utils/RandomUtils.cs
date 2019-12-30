// RandomUtils.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using UnityRandom = UnityEngine.Random;

namespace Pantheon.Utils
{
    public static class RandomUtils
    {
        // List.Random() extension
        public static T Random<T>(this List<T> list)
        {
            return list[UnityRandom.Range(0, list.Count)];
        }

        // T[].Random() extension
        public static T Random<T>(this T[] array)
        {
            return array[UnityRandom.Range(0, array.Length)];
        }

        public static bool OneChanceIn(int x)
        {
            return UnityRandom.Range(0, x + 1) == x;
        }

        /// <summary>
        /// UnityEngine.Random.Range, but inclusive for readability.
        /// </summary>
        public static int RangeInclusive(int min, int max)
            => UnityRandom.Range(min, max + 1);

        public static bool CoinFlip() => UnityRandom.Range(0, 2) == 0;

        public static TEnum EnumRandom<TEnum>()
            where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            // Credits:
            // Stack Overflow users Vivek, Ricardo Nolde, Lisa, Akram Shahda
            Array arr = Enum.GetValues(typeof(TEnum));
            return (TEnum)arr.GetValue(UnityRandom.Range(0, arr.Length));
        }
    }
}
