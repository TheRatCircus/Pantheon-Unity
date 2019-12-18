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

        public Cell HoveredCell { get; private set; }

        private void Update()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Offset tile anchor
            mousePos.x += Cell.TileOffsetX;
            mousePos.y += Cell.TileOffsetY;
            Vector3Int posInt = grid.LocalToCell(mousePos);

            if (Level.TryGetCell(posInt.x, posInt.y, out Cell c))
            {
                HoveredCell = c;
                Vector3 cursorPos = new Vector3(c.Position.x, c.Position.y);
                cursor.transform.position = cursorPos;
            }

            if (Input.GetMouseButtonDown(2))
            {
                LogLocator.Service.Send(HoveredCell.ToString(), Color.white);
            }
        }
    }
}
