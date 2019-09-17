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
        foreach (BodyPart part in actor.Body.Parts)
            if ((part.Prehensile || part.CanMelee) && !part.Dexterous)
                part.Dexterous = true;
    }

    public static void LoseAmbidextrous(Actor actor)
        => throw new System.NotImplementedException();
}
