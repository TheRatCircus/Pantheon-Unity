// SplatFX.cs
// Jerome Martina

using Pantheon.Utils;
using UnityEngine;

/// <summary>
/// Handles sub emitters for splat FX.
/// </summary>
public sealed class SplatFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem parentEmitter = null;

    // Update is called once per frame
    void Update()
    {
        if (RandomUtils.OneChanceIn(1, false))
            parentEmitter.TriggerSubEmitter(0);
    }
}
