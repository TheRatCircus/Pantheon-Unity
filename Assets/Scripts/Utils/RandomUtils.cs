// RandomPick.cs
// Jerome Martina

using UnityEngine;

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

            int chance = Core.Game.PRNG().Next(1, weightSum);
            int runningSum = 0;
            int choice = 0;

            foreach (RandomPickEntry<T> entry in set)
            {
                runningSum += entry.Weight;

                if (chance <= runningSum)
                    return choice;

                choice++;
            }

            throw new System.Exception("RandomPick() returned nothing; set is likely empty.");
        }

        public static T ArrayRandom<T>(T[] array)
        {
            return array[Random.Range(0, array.Length - 1)];
        }
    }
}
