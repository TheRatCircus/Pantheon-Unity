// Hit.cs
// Jerome Martina

namespace Pantheon
{
    /// <summary>
    /// Data passed to an actor's TakeHit() when it receives an incoming hit.
    /// </summary>
    public struct Hit
    {
        public readonly int Damage;

        // Constructor
        public Hit(int minDamage, int maxDamage)
        {
            if (minDamage > maxDamage)
                throw new System.Exception
                    ("A Hit has a higher min than max damage.");

            Damage = UnityEngine.Random.Range(minDamage, maxDamage + 1);
        }
    }
}
