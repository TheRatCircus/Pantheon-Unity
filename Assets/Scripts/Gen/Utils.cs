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
                        c.Terrain = terrain;
        }
    }
}
