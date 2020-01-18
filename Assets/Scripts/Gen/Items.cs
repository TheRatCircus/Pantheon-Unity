// Items.cs
// Jerome Martina

using Pantheon.Content;
using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.Gen
{
    public static class Items
    {
        public static void PopulateItems(Level level)
        {
            int points = 100;
            while (points > 0)
            {
                Vector2Int cell = level.RandomCell(true);
                Entity item;
                if (RandomUtils.OneChanceIn(3)) // Relic
                {
                    item = Relic.MakeRelic();
                    points -= 9; // Relics take a total of 10 points
                }
                else
                {
                    EntityTemplate basic = Assets.Templates[
                        Tables.basicItems.Random()];
                    item = new Entity(basic);
                }

                item.Move(level, cell);
                points--;
            }
        }
    }
}
