// Cursor.cs
// Jerome Martina

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
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Offset tile anchor
            mousePos.x += Level.TileOffsetX;
            mousePos.y += Level.TileOffsetY;
            Vector3Int posInt = grid.LocalToCell(mousePos);

            //if (Level.TryGetCell(posInt.x, posInt.y, out Cell c))
            if (Level.Contains(posInt.x, posInt.y))
            {
                HoveredCell = new Vector2Int(posInt.x, posInt.y);
                Vector3 cursorPos = new Vector3(HoveredCell.x, HoveredCell.y);
                cursor.transform.position = cursorPos;
            }

            if (Input.GetMouseButtonDown(2))
            {
                Locator.Log.Send(HoveredCell.ToString(), Color.white);
            }
        }
    }
}
