// CellTests.cs
// Jerome Martina

using NUnit.Framework;
using Pantheon;
using Pantheon.World;
using UnityEngine;

namespace PantheonTests
{
    public class CellTests
    {
        /// <summary>
        /// Test that a default cell with a default terrain and an actor is
        /// considered walkable.
        /// </summary>
        [Test]
        public void DefaultCellWithActorIsWalkable()
        {
            Cell cell = new Cell(new Vector2Int(0, 0))
            {
                Actor = new Entity("TEST_ENTITY"),
                Terrain = ScriptableObject.CreateInstance<TerrainDefinition>()
            };
            Assert.True(Cell.Walkable(cell));
        }

        /// <summary>
        /// Test that a default cell with an actor is considered blocked.
        /// </summary>
        [Test]
        public void CellWithActorIsBlocked()
        {
            Cell cell = new Cell(new Vector2Int(0, 0))
            {
                Actor = new Entity("TEST_ENTITY")
                {
                    Blocking = true
                },
                Terrain = ScriptableObject.CreateInstance<TerrainDefinition>()
            };
            Assert.True(cell.Blocked);
        }
    }
}
