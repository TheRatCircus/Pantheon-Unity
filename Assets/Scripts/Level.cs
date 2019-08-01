// Level data
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
    // Requisite objects
    public PathFinder pf;

    private Tilemap tilemap;
    public Tile groundTile;
    public Tile wallTile;
    public Tile unknownTile;

    // Map data
    private Cell[,] cells;

    // Attributes of the level
    Vector2Int levelSize;
    Vector2Int spawnPoint;

    // Contents
    public List<Actor> actors;
    Player player;
    public Player _player { get => player; }

    // Properties
    public Cell[,] Cells { get => cells; }
    public Vector2Int LevelSize { get => levelSize; }
    public Tilemap Tilemap { get => tilemap; }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        levelSize = new Vector2Int(64, 64);
        cells = LevelGen.GenerateLevel(ref spawnPoint, levelSize, 15, 5, 16);
    }

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.Find("PlayerCharacter").GetComponent<Player>();
        pf = new PathFinder(this);
        tilemap = transform.GetComponentInChildren<Tilemap>();

        CellDrawer.DrawLevel(this);
        SpawnPlayer();
    }

    // Cell accessor, mostly for validation
    public Cell GetCell(Vector2Int pos)
    {
        if (Contains(pos))
            return cells[pos.x, pos.y];
        else
            throw new System.Exception($"Attempt to access out-of-bounds cell {pos.x}, {pos.y}");
    }

    // Find a random walkable cell in the level
    public Cell GetRandomWalkable()
    {
        Cell cell;
        do
        {
            Vector2Int randomPosition = new Vector2Int();
            randomPosition.x = Random.Range(0, LevelSize.x);
            randomPosition.y = Random.Range(0, LevelSize.y);
            cell = GetCell(randomPosition);
        } while (!cell.IsWalkable());
        return cell;
    }

    // Put the player in their spawn position
    public void SpawnPlayer()
    {
        player.MoveToCell(GetCell(spawnPoint));
        RefreshFOV();
    }

    // Does this Level contain a point?
    public bool Contains(Vector2Int pos)
    {
        if (pos.x < levelSize.x && pos.y < levelSize.y)
            return (pos.x >= 0 && pos.y >= 0);
        else return false;
    }

    // Change visibility and reveal new cells. Only call when a player spawns
    // or moves/is moved
    public void RefreshFOV()
    {
        for (int octant = 0; octant < 8; octant++)
            CellDrawer.DrawCells(this, ShadowOctant(player.Position, octant));
    }

    // Generate an octant of shadows, and return the FOV area to be redrawn
    public List<Cell> ShadowOctant(Vector2Int origin, int octant)
    {
        ShadowLine line = new ShadowLine();
        bool fullShadow = false;
        List<Cell> ret = new List<Cell>();

        for (int row = 0; row < player.FOVRadius; row++)
        {
            // Stop once going out of bounds
            if (!Contains(origin + TransformOctant(row, 0, octant))) break;
            for (int col = 0; col <= row; col++)
            {
                Vector2Int pos = origin + TransformOctant(row, col, octant);
                // Break on this row if going out of bounds
                if (!Contains(pos)) break;

                ret.Add(cells[pos.x, pos.y]);
                // If entire row is known to be in shadow, set this cell to be 
                // in shadow
                if (fullShadow)
                    cells[pos.x, pos.y].Visible = false;
                else
                {
                    Shadow projection = ProjectTile(row, col);
                    
                    // Set the visibility of this tile
                    bool visible = !line.IsInShadow(projection);
                    cells[pos.x, pos.y].Visible = visible;
                    
                    // Add any opaque tiles to the shadow map
                    if (visible && cells[pos.x, pos.y].Opaque)
                    {
                        line.AddShadow(projection);
                        fullShadow = line.IsFullShadow();
                    }
                }
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

    // Transform a point to one of 8 possible octants
    public Vector2Int TransformOctant(int row, int col, int octant)
    {
        switch (octant)
        {
            case 0: return new Vector2Int(col, -row);
            case 1: return new Vector2Int(row, -col);
            case 2: return new Vector2Int(row, col);
            case 3: return new Vector2Int(col, row);
            case 4: return new Vector2Int(-col, row);
            case 5: return new Vector2Int(-row, col);
            case 6: return new Vector2Int(-row, -col);
            case 7: return new Vector2Int(-col, -row);
        }
        throw new System.Exception("Bad octant");
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
}

