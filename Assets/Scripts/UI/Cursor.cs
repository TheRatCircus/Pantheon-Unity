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

        private float tooltipTimeThreshold = 1f;
        private float mouseRestTime;

        private void Update()
        {
            bool mouseAtRest = false;

            if (!Input.anyKeyDown &&
                Input.GetAxis("MouseX") == 0 &&
                Input.GetAxis("MouseY") == 0)
            {
                mouseAtRest = true;
                mouseRestTime += Time.deltaTime;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Offset tile anchor
            mousePos.x += Level.TileOffsetX;
            mousePos.y += Level.TileOffsetY;
            Vector3Int posInt = grid.LocalToCell(mousePos);

            if (!Level.Contains(posInt.x, posInt.y))
                return;

            HoveredCell = new Vector2Int(posInt.x, posInt.y);
            Vector3 cursorPos = new Vector3(HoveredCell.x, HoveredCell.y);
            cursor.transform.position = cursorPos;

            if (mouseAtRest && mouseRestTime >= tooltipTimeThreshold)
            {
#if DEBUG_MOUSEOVER
                Locator.Log.Send(
                    $"{Level.ChunkContaining(HoveredCell.x, HoveredCell.y)}",
                    Color.magenta);
#endif
                mouseRestTime = 0f;
            }
        }
    }
}
