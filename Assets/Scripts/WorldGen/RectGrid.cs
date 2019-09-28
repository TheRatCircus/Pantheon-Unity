// RectGrid.cs
// Jerome Martina

using System.Collections.Generic;
using Pantheon.World;
using static Pantheon.WorldGen.Layout;
using UnityEngine;

namespace Pantheon.WorldGen
{
    public static class RectGrid
    {
        public static void DrawGrid(Level level, Vector2Int rectSize)
        {
            if (rectSize.x >= level.LevelSize.x
                || rectSize.y >= level.LevelSize.y)
                throw new System.ArgumentException
                    ("Rectangle size cannot meet or exceed level size.");

            List<LevelRect> rects = new List<LevelRect>();

            int numXRectangles = level.LevelSize.x / rectSize.x;
            int numYRectangles = level.LevelSize.y / rectSize.y;

            Vector2Int position = new Vector2Int(0, 0);

            for (int x = 0; x < numXRectangles; x++)
            {
                for (int y = 0; y < numYRectangles; y++)
                {
                    LevelRect rect = new LevelRect(
                        position, rectSize);
                    rects.Add(rect);
                    position.x += rectSize.x;
                }
                position.x = 0;
                position.y += rectSize.y; 
            }

            //foreach (LevelRect rect in rects)
            //{
            //    Debug.Visualisation.MarkPos(new Vector2Int(rect.x1, rect.y1), 10);
            //    Debug.Visualisation.MarkPos(new Vector2Int(rect.x2, rect.y2), 10);
            //}

            //RoomsFromGrid(level, rects);
        }

        public static void RoomsFromGrid(Level level,
            List<LevelRect> rects)
        {
            foreach (LevelRect rect in rects)
                GenerateRoom(level, rect, TerrainType.StoneWall,
                    TerrainType.StoneFloor);
        }
    }
}