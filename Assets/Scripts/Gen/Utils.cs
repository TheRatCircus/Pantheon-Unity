// Utils.cs
// Jerome Martina

using Pantheon.Content;
using Pantheon.World;

namespace Pantheon.Gen
{
    public static class Utils
    {
        public static void FillRectTerrain(Level level, LevelRect rect,
            TerrainDefinition terrain)
        {
            for (int x = rect.x1; x < rect.x2; x++)
                for (int y = rect.y1; y < rect.y2; y++)
                    if (level.TryGetCell(x, y, out Cell c))
                    {
                        level.SetTerrain(x, y, terrain);
                    }
        }

        public static void Enclose(Level level, LevelRect rect,
            TerrainDefinition terrain)
        {
            for (int x = rect.x1; x <= rect.x2; x++)
                for (int y = rect.y1; y <= rect.y2; y++)
                {
                    if (x == rect.x1)
                    {
                        level.SetTerrain(x, y, terrain);
                        continue;
                    }
                    else if (x == rect.x2)
                    {
                        level.SetTerrain(x, y, terrain);
                        continue;
                    }
                    else
                    {
                        if (y == rect.y1)
                        {
                            level.SetTerrain(x, y, terrain);
                            continue;
                        }
                        else if (y == rect.y2)
                        {
                            level.SetTerrain(x, y, terrain);
                            continue;
                        }
                    }
                }
        }

        public static void Enclose(Level level, TerrainDefinition terrain)
        {
            for (int x = 0; x < level.Size.x; x++)
                for (int y = 0; y < level.Size.y; y++)
                {
                    if (x == 0)
                    {
                        level.SetTerrain(x, y, terrain);
                        continue;
                    }
                    else if (x == level.Size.x - 1)
                    {
                        level.SetTerrain(x, y, terrain);
                        continue;
                    }
                    else
                    {
                        if (y == 0)
                        {
                            level.SetTerrain(x, y, terrain);
                            continue;
                        }
                        else if (y == level.Size.y - 1)
                        {
                            level.SetTerrain(x, y, terrain);
                            continue;
                        }
                    }
                }
        }
    }
}
