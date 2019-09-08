// Data passed to an actor's TakeHit() when it receives an incoming hit
public class Hit
{
    private readonly int damage;
    public int Damage => damage;

    // Constructor
    public Hit(int minDamage, int maxDamage)
        => damage = UnityEngine.Random.Range(minDamage, maxDamage + 1);
}
