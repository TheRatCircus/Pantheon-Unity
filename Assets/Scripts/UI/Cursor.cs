// Cursor.cs
// Jerome Martina

using Pantheon.Utils;
using Pantheon.World;
using UnityEngine;

namespace Pantheon.UI
{
    public sealed class Cursor : MonoBehaviour
    {
        [SerializeField] private GameObject cursor = default;
        [SerializeField] private Grid grid = default;
        public Level Level { get; set; }

        public Vector2Int HoveredCell { get; private set; }

        private void Update()
        {
            // TODO: Cache main camera
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Offset tile anchor
            mousePos.x += Level.TileOffset;
            mousePos.y += Level.TileOffset;
            Vector2Int posInt = (Vector2Int)grid.LocalToCell(mousePos);

            if (Level.Contains(posInt))
            {
                HoveredCell = posInt;
                Vector3 cursorPos = HoveredCell.ToVector3();
                cursor.transform.position = cursorPos;
            }

            if (Input.GetMouseButtonDown(2))
            {
                Locator.Log.Send(Level.CellToString(HoveredCell), Color.white);
            }
        }
    }
}
