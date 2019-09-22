// RandomPick.cs
// Jerome Martina

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pantheon.Core;

namespace Pantheon.Utils
{
    /// <summary>
    /// Utilies for random picking.
    /// </summary>
    public static class RandomUtils
    {
        /// <summary>
        /// An entry in a set from which something random can be picked by weight.
        /// </summary>
        /// <typeparam name="T">Value type.</typeparam>
        public struct RandomPickEntry<T>
        {
            public int Weight; // 0...512
            public T Value;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="weight">The relative weight used to pick this entry.</param>
            /// <param name="value">The value picked.</param>
            public RandomPickEntry(int weight, T value)
            {
                Weight = weight;
                Value = value;
            }

            /// <summary>
            /// Get the sum of the weights of a set of type RandomPickEntry.
            /// </summary>
            /// <param name="set">Set of type RandomPickEntry.</param>
            /// <returns>The sum of all the weights in set.</returns>
            public static int WeightSum(RandomPickEntry<T>[] set)
            {
                int weightSum = 0;
                foreach (RandomPickEntry<T> entry in set)
                    weightSum += entry.Weight;

                return weightSum;
            }
        }

        public static T RandomPick<T>(this RandomPickEntry<T>[] set, bool seeded)
        {
            int weightSum = RandomPickEntry<T>.WeightSum(set);

            int chance;
            if (seeded)
                chance = Game.PRNG.Next(1, weightSum);
            else
                chance = UnityEngine.Random.Range(1, weightSum);

            int runningSum = 0;
            int choice = 0;

            foreach (RandomPickEntry<T> entry in set)
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

        public static bool OneChanceIn(int x) => RangeInclusive(0, x) == x;

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
    }
}
