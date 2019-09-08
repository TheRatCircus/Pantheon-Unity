// ItemWeights.cs
// Jerome Martina

using static Pantheon.Utils.RandomUtils;

/// <summary>
/// Assigning weighting to items for random picking during level gen.
/// </summary>
public static class ItemWeights
{
    public static T RandomWeighted<T>(RandomPickEntry<T>[] set)
        => set[RandomPick(set)].Value;

    public static RandomPickEntry<ScrollType>[] ScrollWeights =
    {
        new RandomPickEntry<ScrollType>(512, ScrollType.MagicBullet)
    };

    public static RandomPickEntry<FlaskType>[] FlaskWeights =
    {
        new RandomPickEntry<FlaskType>(512, FlaskType.Healing)
    };
}
