// Handler for advanced attack GUI
using UnityEngine;
using UnityEngine.UI;

public class AdvancedAttackUI : MonoBehaviour
{
    public Player player;
    public GameObject advancedAttackUI;

    public Text targetText;

    private void Start()
    {
        player.OnAdvancedAttackEvent += AdvancedAttack;
    }

    private void AdvancedAttack(Actor target)
    {
    }
}
