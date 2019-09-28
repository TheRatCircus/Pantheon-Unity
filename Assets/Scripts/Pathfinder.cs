// Pathfinder.cs
// Credit to Sebastian Lague

using Pantheon.World;
using System.Collections.Generic;
using UnityEngine;

namespace Pantheon
{
    /// <summary>
    /// Pathfinding data and functions for a level.
    /// </summary>
    public sealed class Pathfinder
    {
        private Level level;
        private Node[,] map;

        public Pathfinder(Level level)
        {
            this.level = level;
            map = new Node[level.LevelSize.x, level.LevelSize.y];
            GetMap(level);
        }

        // Generate the pathfinding node map based on the level data
        void GetMap(Level level)
        {
            for (int x = 0; x < level.LevelSize.x; x++)
                for (int y = 0; y < level.LevelSize.y; y++)
                    map[x, y] = new Node(level.Map[x, y].Blocked,
                        new Vector2Int(x, y));
        }

        // Get a Node's neighbours if they are valid
        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                    if (x == 0 && y == 0)
                        continue;
                    else
                    {
                        int checkX = node.Position.x + x;
                        int checkY = node.Position.y + y;

                        if (checkX >= 0 && checkX < map.GetLength(0) &&
                            checkY >= 0 && checkY <= map.GetLength(1))
                        {
                            neighbours.Add(map[checkX, checkY]);
                        }
                    }
            return neighbours;
        }

        // Get a path of cells from the pathfinder
        public List<Cell> GetCellPath(Vector2Int startPos,
            Vector2Int targetPos)
        {
            List<Cell> cellPath = new List<Cell>();
            List<Node> nodes = FindPath(startPos, targetPos);

            if (nodes != null)
                foreach (Node n in nodes)
                    cellPath.Add(NodeToCell(n));

            return cellPath;
        }

        // Find a path to a target from a position
        List<Node> FindPath(Vector2Int startPos, Vector2Int targetPos)
        {
            Node startNode = map[startPos.x, startPos.y];
            Node targetNode = map[targetPos.x, targetPos.y];

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost
                        || openSet[i].FCost == currentNode.FCost
                        && openSet[i].HCost < currentNode.HCost)
                    {
                        currentNode = openSet[i];
                    }
                }
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                    return RetracePath(startNode, targetNode);

                foreach (Node neighbour in GetNeighbours(currentNode))
                {
                    if (neighbour.Blocked || closedSet.Contains(neighbour))
                        continue;

                    int newMoveCostToNeighbour = currentNode.GCost + 
                        GetDistance(currentNode, neighbour);
                    if (newMoveCostToNeighbour < neighbour.GCost ||
                        !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newMoveCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);
                        neighbour.Parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
            return null;
        }

        // Trace a series of nodes back to the start to create a complete path
        List<Node> RetracePath(Node startNode, Node targetNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = targetNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }

        // Get a cell given a pathfinding node
        Cell NodeToCell(Node node) => level.GetCell(node.Position);

        // Get equivalent distance between two nodes
        int GetDistance(Node a, Node b)
        {
            int dX = Mathf.Abs(a.Position.x - b.Position.x);
            int dY = Mathf.Abs(a.Position.y - b.Position.y);

            if (dX > dY)
                return 14 * dY + 10 * (dX - dY);
            else
                return 14 * dX + 10 * (dY - dX);
        }
    }
}

