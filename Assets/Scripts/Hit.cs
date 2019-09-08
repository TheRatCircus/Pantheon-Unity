// Hit.cs
// Jerome Martina

/// <summary>
/// Data passed to an actor's TakeHit() when it receives an incoming hit.
/// </summary>
public class Hit
{
    private readonly int damage;
    public int Damage => damage;

    // Constructor
    public Hit(int minDamage, int maxDamage)
        => damage = UnityEngine.Random.Range(minDamage, maxDamage + 1);
}
