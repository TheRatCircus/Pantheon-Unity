// Hit.cs
// Jerome Martina

/// <summary>
/// Data passed to an actor's TakeHit() when it receives an incoming hit.
/// </summary>
public struct Hit
{
    public readonly int Damage;

    // Constructor
    public Hit(int minDamage, int maxDamage)
        => Damage = UnityEngine.Random.Range(minDamage, maxDamage + 1);
}
