// Traits.cs
// Jerome Martina

using System.Collections.Generic;
using Pantheon.Actors;
using static TraitEffects;

/// <summary>
/// Get traits from here.
/// </summary>
public static class Traits
{
    public static Trait Ambidextrous = new Trait(
        "Ambidextrous",
        ApplyAmbidextrous,
        LoseAmibdextrous);
    public static Trait Adrenaline = new Trait(
        "Adrenaline",
        null,
        null);

    /// <summary>
    /// Used for lookup via console.
    /// </summary>
    public static Dictionary<string, Trait> traitsLookup
        = new Dictionary<string, Trait>()
    {
            { "ambidextrous", Ambidextrous },
            { "adrenaline", Adrenaline }
    };
}