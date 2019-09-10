// DebugInfo.cs
// Jerome Martina

using UnityEngine;
using UnityEngine.UI;
using Pantheon.Core;
using Pantheon.Actors;

public class DebugInfo : MonoBehaviour
{
    [SerializeField] private Text activeActor = null;

    private void Start()
    {
        Game.instance.ActorDebugEvent += UpdateActiveActor;
    }

    void UpdateActiveActor(Actor actor)
    {
        activeActor.text = $"{actor.ActorName} ({actor.Energy})";
    }
}
