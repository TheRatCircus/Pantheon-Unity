// GenericRandomPick.cs
// Jerome Martina

using System;

namespace Pantheon.Utils
{
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

        public static T Pick(GenericRandomPick<T>[] set)
        {
            int weightSum = WeightSum(set);

            int chance;
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
    }
}
