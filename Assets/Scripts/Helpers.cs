// Helper functions, mostly math and cells
using UnityEngine;

public static class Helpers
{
    // Convert a position on a grid to a Vector3
    public static Vector3 GridToVector3(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x, gridPos.y);
    }
}
