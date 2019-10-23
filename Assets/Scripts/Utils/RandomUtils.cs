// RandomPick.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using System.Linq;
using Pantheon.Core;
using UnityEngine;

namespace Pantheon.Utils
{
    /// <summary>
    /// Utilies for random picking.
    /// </summary>
    public static class RandomUtils
    {
        [Serializable]
        public struct SerializableRandomPick
        {
            [SerializeField] [Range(0, 512)] private int weight;
            [SerializeField] private UnityEngine.Object obj;

            public int Weight => weight;
            public UnityEngine.Object Obj => obj;

            private SerializableRandomPick(int weight, UnityEngine.Object obj)
            {
                this.weight = weight;
                this.obj = obj;
            }

            public static int WeightSum(SerializableRandomPick[] set)
            {
                int weightSum = 0;
                foreach (SerializableRandomPick entry in set)
                    weightSum += entry.weight;

                return weightSum;
            }
        }

        public struct GenericRandomPick<T>
        {
            public readonly int Weight; // 0...512
            public readonly T Value;

            public GenericRandomPick(int weight, T value)
            {
                if (weight < 0 || weight > 512)
                    throw new ArgumentException(
                        "Constrain weight between 0 and 512.");

                Weight = weight;
                Value = value;
            }

            /// <summary>
            /// Get the sum of the weights of a set of type RandomPickEntry.
            /// </summary>
            /// <param name="set">Set of type RandomPickEntry.</param>
            /// <returns>The sum of all the weights in set.</returns>
            public static int WeightSum(GenericRandomPick<T>[] set)
            {
                int weightSum = 0;
                foreach (GenericRandomPick<T> entry in set)
                    weightSum += entry.Weight;

                return weightSum;
            }
        }

        public static T RandomPick<T>(this GenericRandomPick<T>[] set, bool seeded)
        {
            int weightSum = GenericRandomPick<T>.WeightSum(set);

            int chance;
            if (seeded)
                chance = Game.PRNG.Next(1, weightSum);
            else
                chance = UnityEngine.Random.Range(1, weightSum);

            int runningSum = 0;
            int choice = 0;

            foreach (GenericRandomPick<T> entry in set)
            {
                runningSum += entry.Weight;

                if (chance <= runningSum)
                    return set[choice].Value;

                choice++;
            }

            throw new Exception("Returned nothing; set is likely empty.");
        }
        
        public static bool CoinFlip(bool seeded)
        {
            if (seeded)
                return Game.PRNG.Next(2) == 0;
            else
                return UnityEngine.Random.Range(0, 2) == 0;
        }

        public static bool OneChanceIn(int x, bool seeded)
        {
            if (seeded)
                return Game.PRNG.Next(0, x + 1) == x;
            else
                return RangeInclusive(0, x) == x;
        }

        /// <summary>
        /// UnityEngine.Random.Range, but inclusive, and thus more readable.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RangeInclusive(int min, int max)
            => UnityEngine.Random.Range(min, max + 1);

        // Random() extension method for arrays
        public static T Random<T>(this T[] array, bool seeded)
        {
            if (seeded)
                return array[Game.PRNG.Next(array.Length)];
            else
                return array[UnityEngine.Random.Range(0, array.Length)];
        }

        // List.Random() extension method
        public static T Random<T>(this List<T> list, bool seeded)
        {
            if (seeded)
                return list[Game.PRNG.Next(list.Count)];
            else
                return list[UnityEngine.Random.Range(0, list.Count)];
        }

        // Dictionary.Random() extension method
        public static TValue Random<TKey, TValue>(
            this IDictionary<TKey, TValue> dict, bool seeded)
        {
            // Credit to StriplingWarrior on Stack Overflow
            List<TValue> values = Enumerable.ToList(dict.Values);
            return values.Random(seeded);
        }

        public static TEnum EnumRandom<TEnum>(bool seeded)
            where TEnum : struct, IConvertible, IComparable, IFormattable
        {
             // Credits:
             // Stack Overflow users Vivek, Ricardo Nolde, Lisa, Akram Shahda.
            Array arr = Enum.GetValues(typeof(TEnum));
            if (seeded)
                return (TEnum)arr.GetValue(Game.PRNG.Next(arr.Length));
            else
                return (TEnum)arr.GetValue(UnityEngine.Random.Range(0, arr.Length));
        }

        public static void Shuffle<T>(this IList<T> list, bool seeded)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = 0;

                if (seeded)
                    k = Game.PRNG.Next(n + 1);
                else
                    k = UnityEngine.Random.Range(0, n + 1);

                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
