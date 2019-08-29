// Level data
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
    // Requisite objects
    public PathFinder pf;

    // This level's tilemaps
    public Tilemap terrainTilemap;
    public Tilemap featureTilemap;
    public Tilemap itemTilemap;
    public Tilemap targettingTilemap;

    public Tile stairsDown;
    public Tile stairsUp;
    public Tile unknownTile;

    public GameObject enemyPrefab;

    // Map data
    private Cell[,] cells;

    // Attributes of the level
    Vector2Int levelSize;

    // Contents
    public List<Actor> actors;

    // Properties
    public Cell[,] Cells { get => cells; }
    public Vector2Int LevelSize { get => levelSize; }

    // Initialize this level
    public void Initialize(bool first)
    {
        levelSize = new Vector2Int(64, 64);
        cells = LevelGen.GenerateLevel(levelSize, 15, 5, 16);
        CellDrawer.DrawLevel(this);

        pf = new PathFinder(this);

        if (first)
            SpawnPlayer();
        SpawnEnemies();
        for (int i = 0; i < 10; i++)
        {
            RandomFloor().Items.Add(new Item(Database.GetFlask(FlaskType.FlaskHealing)));
            RandomFloor().Items.Add(new Item(Database.GetScroll(ScrollType.ScrollMagicBullet)));
        }
    }

    // Cell accessor, mostly for validation
    public Cell GetCell(Vector2Int pos)
    {
        if (Contains(pos))
            return cells[pos.x, pos.y];
        else
            throw new System.Exception($"Attempt to access out-of-bounds cell {pos.x}, {pos.y}");
    }

    // Put the player in their spawn position
    public void SpawnPlayer()
    {
        actors.Add(Game.instance.player1);
        Game.instance.player1.MoveToCell(RandomFloor());
        RefreshFOV();
    }

    // Spawn some enemies at random about the dungeon, but never too close to
    // the player spawn point
    public void SpawnEnemies()
    {
        for (int i = 0; i < 10; i++)
        {
            Cell cell;
            int attempts = 0;
            do
            {
                if (attempts > 100)
                    throw new System.Exception("Attempt to generate new enemy position failed");
                cell = RandomFloor();
                attempts++;
            } while (Distance(cell, Game.instance.player1._cell) <= 7
            || cell._actor != null);
            MakeEntity.MakeEnemyAt(enemyPrefab, this, cell);
        }
    }

    #region Helpers

    // Find a random walkable cell in the level
    public Cell RandomFloor()
    {
        Cell cell;
        do
        {
            Vector2Int randomPosition = new Vector2Int
            {
                x = Random.Range(0, LevelSize.x),
                y = Random.Range(0, LevelSize.y)
            };
            cell = GetCell(randomPosition);
        } while (!cell.IsWalkable());
        return cell;
    }

    // Get a random floor beyond a certain distance from another point
    public Cell RandomFloorAwayFrom(Cell other, int distance)
    {
        Cell cell;
        do
        {
            Vector2Int randomPosition = new Vector2Int
            {
                x = Random.Range(0, LevelSize.x),
                y = Random.Range(0, LevelSize.y)
            };
            cell = GetCell(randomPosition);
        } while (!cell.IsWalkable() || Distance(cell, other) <= distance);
        return cell;
    }

    // Get the distance between two cells on this level
    public int Distance(Cell a, Cell b)
    {
        int dx = b.Position.x - a.Position.x;
        int dy = b.Position.y - a.Position.y;

        return (int)Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
    }

    // Does this Level contain a point?
    public bool Contains(Vector2Int pos)
    {
        if (pos.x < levelSize.x && pos.y < levelSize.y)
            return (pos.x >= 0 && pos.y >= 0);
        else return false;
    }

    // Check if one cell is adjacent to another
    public bool AdjacentTo(Cell a, Cell b)
    {
        return Distance(a, b) <= 1;
    }

    // Get an adjacent cell given a direction
    public Cell GetAdjacentCell(Cell origin, Vector2Int delta)
    {
        if (delta.x == 0 && delta.y == 0)
            throw new System.ArgumentException("Level.GetAdjacentCell requires a non-zero delta");
        else if (delta.x > 1 || delta.y > 1)
            Debug.LogWarning("GetAdjacentCell was passed a delta with a value greater than one");

        delta.Clamp(new Vector2Int(-1, -1), new Vector2Int(1, 1));

        Vector2Int newCellPos = origin.Position + delta;
        return GetCell(newCellPos);
    }

    #endregion

    #region FOV

    // Change visibility and reveal new cells. Only call when a player spawns
    // or moves/is moved
    public void RefreshFOV()
    {
        List<Cell> allRefreshed = new List<Cell>();
        for (int octant = 0; octant < 8; octant++)
        {
            List<Cell> refreshed = ShadowOctant(Game.instance.player1.Position, octant);
            CellDrawer.DrawCells(this, refreshed);
            allRefreshed.AddRange(refreshed);
        }
        Game.instance.player1.UpdateVisibleCells(allRefreshed);
    }

    // Coordinates used to transform a point in an octant
    static Vector2Int[,] octantCoordinates = new Vector2Int[,]
    {
        { new Vector2Int(0, -1), new Vector2Int(1, 0) },
        { new Vector2Int(1, 0), new Vector2Int(0, -1) },
        { new Vector2Int(1, 0), new Vector2Int(0, 1) },
        { new Vector2Int(0, 1), new Vector2Int(1, 0) },
        { new Vector2Int(0, 1), new Vector2Int(-1, 0) },
        { new Vector2Int(-1, 0), new Vector2Int(0, 1) },
        { new Vector2Int(-1, 0), new Vector2Int(0, -1) },
        { new Vector2Int(0, -1), new Vector2Int(-1, 0) }
    };

    // Generate an octant of shadows, and return the FOV area to be redrawn
    public List<Cell> ShadowOctant(Vector2Int origin, int octant)
    {
        // Increments based off of octantCoordinates
        var rowInc = octantCoordinates[octant, 0];
        var colInc = octantCoordinates[octant, 1];

        ShadowLine line = new ShadowLine();
        bool fullShadow = false;
        List<Cell> ret = new List<Cell>();

        for (int row = 0; row < 30; row++)
        {
            // Record position
            Vector2Int pos = origin + (rowInc * row);
            // Stop once going out of bounds
            if (!Contains(pos)) break;

            bool pastMaxDistance = false;
            for (int col = 0; col <= row; col++)
            {
                // Break on this row if going out of bounds
                if (!Contains(pos)) break;
                // Add new cells to list of updated cells
                ret.Add(cells[pos.x, pos.y]);
                // Visibility fall off over distance
                int fallOff = 255;

                // If entire row is known to be in shadow, set this cell to be 
                // in shadow
                if (fullShadow || pastMaxDistance)
                    cells[pos.x, pos.y].SetVisibility(false, fallOff);
                else
                {
                    fallOff = 0;
                    float distance = Vector2.Distance(origin, pos);
                    if (distance > Game.instance.player1.FOVRadius)
                    {
                        fallOff = 255;
                        pastMaxDistance = true;
                    }
                    else
                    {
                        float normalized = distance / Game.instance.player1.FOVRadius;
                        normalized = Mathf.Pow(normalized, 2);
                        fallOff = (int)(normalized * 255);
                    }
                    Shadow projection = ProjectTile(row, col);
                    
                    // Set the visibility of this tile
                    bool visible = !line.IsInShadow(projection);
                    cells[pos.x, pos.y].SetVisibility(visible, fallOff);
                    
                    // Add any opaque tiles to the shadow map
                    if (visible && cells[pos.x, pos.y].Opaque)
                    {
                        line.AddShadow(projection);
                        fullShadow = line.IsFullShadow();
                    }
                }
                pos += colInc;
                if (!Contains(pos)) break;
            }
        }
        return ret;
    }

    // Creates a Shadow that corresponds to the projected silhouette of the
    // tile at row, col
    Shadow ProjectTile(float row, float col)
    {
        float rowF = row;
        float colF = col;

        float topLeft = colF / (rowF + 2);
        float bottomRight = (colF + 1) / (rowF + 1);

        return new Shadow(topLeft, bottomRight, 
            new Vector2(col, row + 2), 
            new Vector2(col + 1, row + 1));
    }

    // Generate a line of shadows
    public class ShadowLine
    {
        public readonly List<Shadow> Shadows = new List<Shadow>();

        // Is this projection within shadow?
        public bool IsInShadow(Shadow projection)
        {
            foreach (Shadow shadow in Shadows)
            {
                if (shadow.Contains(projection))
                    return true;
            }
            return false;
        }

        // Is this line in full shadow?
        public bool IsFullShadow()
        {
            return 
                Shadows.Count == 1 &&
                Shadows[0].Start == 0 &&
                Shadows[0].End == 1;
        }

        // Determine where to add a new shadow to the list
        public void AddShadow(Shadow shadow)
        {
            int index = 0;
            
            for (; index < Shadows.Count; index++)
                // Stop when hitting the insertion point
                if (Shadows[index].Start >= shadow.Start) break;

            // Check if new shadow overlaps previous or next
            Shadow overlappingPrevious = null;
            if (index > 0 && Shadows[index - 1].End > shadow.Start)
                overlappingPrevious = Shadows[index - 1];

            Shadow overlappingNext = null;
            if (index < Shadows.Count && Shadows[index].Start < shadow.End)
                overlappingNext = Shadows[index];

            // Insert and unify with overlapping shadows
            if (overlappingNext != null)
            {
                if (overlappingPrevious != null)
                {
                    // Overlaps both, so unify one and delete the other
                    overlappingPrevious.End = overlappingNext.End;
                    overlappingPrevious.EndPos = overlappingNext.EndPos;
                    Shadows.RemoveAt(index);
                }
                else
                {
                    // Only overlaps the next shadow, so unify it with that
                    overlappingNext.Start = shadow.Start;
                    overlappingNext.StartPos = shadow.StartPos;
                }
            }
            else
            {
                if (overlappingPrevious != null)
                {
                    // Overlaps the previous one, so unify it with that
                    overlappingPrevious.End = shadow.End;
                    overlappingPrevious.EndPos = shadow.EndPos;
                }
                else
                {
                    // Does not overlap with anything, so insert
                    Shadows.Insert(index, shadow);
                }
            }
        }
    }

    // Represents the 1D projection of a 2D shadow onto a normalized line. In
    // other words, a range from 0.0 to 1.0
    public class Shadow
    {
        float start;
        float end;
        Vector2 startPos;
        Vector2 endPos;

        public float Start { get => start; set => start = value; }
        public float End { get => end; set => end = value; }
        public Vector2 EndPos { get => endPos; set => endPos = value; }
        public Vector2 StartPos { get => startPos; set => startPos = value; }

        public Shadow(float start, float end, Vector2 startPos, Vector2 endPos)
        {
            this.start = start;
            this.end = end;
            this.startPos = startPos;
            this.endPos = endPos;
        }

        public override string ToString() => $"{start}-{end}";

        public bool Contains(Shadow other)
        {
            return start <= other.Start && end >= other.End;
        }
    }

    #endregion
}
