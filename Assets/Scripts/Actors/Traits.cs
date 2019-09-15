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
    public static Dictionary<TraitRef, Trait> _traits = new Dictionary<TraitRef, Trait>()
    {
        { TraitRef.Ambidexterous, new Trait("Ambidextrous", ApplyAmbidextrous, LoseAmbidextrous) },
        { TraitRef.Adrenaline, new Trait("Adrenaline", null, null) },
        { TraitRef.Endemic, new Trait("Endemic", null, null) }
    };
}

/// <summary>
/// Provides constants for retrieval from Traits._traits.
/// </summary>
public enum TraitRef
{
    Ambidexterous,
    Adrenaline,
    Endemic
}