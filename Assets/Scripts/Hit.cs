// Data passed to an actor's TakeHit() when it receives an incoming hit
public class Hit
{
    public int Damage;

    // Constructor
    public Hit(int minDamage, int maxDamage)
    {
        Damage = UnityEngine.Random.Range(minDamage, maxDamage + 1);
    }
}
