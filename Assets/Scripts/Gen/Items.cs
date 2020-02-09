// Items.cs
// Jerome Martina

using Pantheon.Content;
using Pantheon.Utils;
using Pantheon.World;

namespace Pantheon.Gen
{
    public static class Items
    {
        public static void PopulateItems(Level level)
        {
            int points = 100;
            while (points > 0)
            {
                Cell cell = level.RandomCell(true);
                Entity item;
                if (RandomUtils.OneChanceIn(3)) // Relic
                {
                    item = Relic.MakeRelic();
                    points -= 9; // Relics take a total of 10 points
                }
                else
                {
                    EntityTemplate basic = Assets.GetTemplate(
                        Tables.basicItems.Random());
                    item = new Entity(basic);
                }

                item.Move(level, cell);
                points--;
            }
        }
    }
}
