// CellTests.cs
// Jerome Martina

using NUnit.Framework;
using Pantheon;
using Pantheon.Content;
using Pantheon.World;
using UnityEngine;

namespace PantheonTests
{
    internal static class LevelTests
    {
        [Test]
        public static void ChunkContainingTest()
        {
            Level level = new Level(
                new Vector2Int(Chunk.Width * 10, Chunk.Height * 10));
            Vector2Int centre = new Vector2Int(0, 0);
            Vector2Int northEast = new Vector2Int(1, 1);
            Assert.True(level.ChunkContaining(
                0, 0).Position == centre);
            Assert.True(level.ChunkContaining(
                Chunk.Width + 1, Chunk.Height + 1).Position == northEast);
        }
    }
}
