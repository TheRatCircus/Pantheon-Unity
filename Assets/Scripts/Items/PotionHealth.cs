// Healing potion
using UnityEngine;

[CreateAssetMenu(fileName = "PotionHealth", menuName = "Items/Potion")]
public class PotionHealth : Potion
{
    // Effect activated when this potion is drunk
    // Restore quarter of user's HP
    public override void OnUse(Actor user)
    {
        user.Health += user.MaxHealth / 4;
        if (user is Player)
        {
            GameLog.Send("You drink the potion...and feel rejuvenated!", MessageColour.White);
        }
    }
}
