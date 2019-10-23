// TraitEffects.cs
// Jerome Martina

using Pantheon.Actors;

/// <summary>
/// Functions assigned as callbacks to traits.
/// </summary>
public static class TraitEffects
{
    public static void ApplyAmbidextrous(Actor actor)
    {
        foreach (Appendage app in actor.Body.Parts)
            if ((app.Prehensile || app.CanMelee) && !app.Dexterous)
                app.Dexterous = true;
    }

    public static void LoseAmbidextrous(Actor actor)
        => throw new System.NotImplementedException();
}
