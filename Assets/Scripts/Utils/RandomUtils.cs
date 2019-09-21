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

        public static int RandomPick<T>(RandomPickEntry<T>[] set)
        {
            int weightSum = RandomPickEntry<T>.WeightSum(set);

            int chance = Game.PRNG.Next(1, weightSum);
            int runningSum = 0;
            int choice = 0;

            foreach (RandomPickEntry<T> entry in set)
            {
                runningSum += entry.Weight;

                if (chance <= runningSum)
                    return choice;

                choice++;
            }

            throw new System.Exception
                ("RandomPick() returned nothing; set is likely empty.");
        }

        public static T ArrayRandom<T>(T[] array)
            => array[Game.PRNG.Next(array.Length)];

        public static bool CoinFlip() => RangeInclusive(0, 1) == 0;

        public static bool OneChanceIn(int x) => RangeInclusive(0, x) == x;

        /// <summary>
        /// UnityEngine.Random.Range, but inclusive, and thus more readable.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RangeInclusive(int min, int max)
            => UnityEngine.Random.Range(min, max + 1);

        public static T ListRandom<T>(List<T> list)
        {
            int index = Game.PRNG.Next(list.Count);
            return list[index];
        }

        /// <summary>
        /// Returns a random value from a dictionary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static TValue DictRandom<TKey, TValue>
            (IDictionary<TKey, TValue> dict)
        {
            // Credit to StriplingWarrior on Stack Overflow
            List<TValue> values = Enumerable.ToList(dict.Values);
            return ListRandom(values);
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
