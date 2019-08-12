// An effect played out at the end of the turn
using System.Collections;
using UnityEngine;

public abstract class EndTurnEffect : MonoBehaviour
{
    // Carry out this end-of-turn effect
    public abstract IEnumerator DoEffect();
}
