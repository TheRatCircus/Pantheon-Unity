// Pathfinder.cs
// Courtesy of Sebastian Lague

using Pantheon.World;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Pantheon
{
    [Serializable]
    public sealed class Pathfinder
    {
        private Dictionary<Vector2Int, Node> map;

        public Pathfinder(Level level)
        {
            map = new Dictionary<Vector2Int, Node>(level.Map.Count);
            GetMap(level);
        }

        void GetMap(Level level)
        {
            foreach (Cell c in level.Map.Values)
                map.Add(c.Position, new Node(c));
        }

        public HashSet<Node> GetNeighbours(Node node)
        {
            Profiler.BeginSample("Pathfinder: GetNeighbours()");
            HashSet<Node> neighbours = new HashSet<Node>();

            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                    if (x == 0 && y == 0)
                        continue;
                    else
                    {
                        if (map.TryGetValue(new Vector2Int(node.Position.x + x,
                            node.Position.y + y), out Node n))
                            neighbours.Add(n);
                    }

            Profiler.EndSample();
            return neighbours;
        }

        public List<Cell> CellPathList(Vector2Int startPos,
            Vector2Int targetPos)
        {
            Profiler.BeginSample("Pathfinding");
            List<Node> nodes = FindPath(startPos, targetPos);
            Profiler.EndSample();

            if (nodes == null)
                return new List<Cell>(0);

            List<Cell> cellPath = new List<Cell>(nodes.Count);

            foreach (Node n in nodes)
                cellPath.Add(n.Cell);

            return cellPath;
        }

        List<Node> FindPath(Vector2Int startPos, Vector2Int targetPos)
        {
            if (!map.TryGetValue(startPos, out Node startNode))
                throw new ArgumentException(
                    $"No node at {startPos}.");
            if (!map.TryGetValue(targetPos, out Node targetNode))
                throw new ArgumentException(
                    $"No node at {targetPos}.");

            if (targetNode.Cell.Blocked)
                return null;

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            int j = 0;
            while (openSet.Count > 0)
            {
                if (j >= 10000)
                    throw new Exception(
                        $"Pathfinder iterated {j} times.");

                Node currentNode = openSet[0];
                Profiler.BeginSample("Pathfind: Cost Evaluation");
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost
                        || openSet[i].FCost == currentNode.FCost
                        && openSet[i].HCost < currentNode.HCost)
                    {
                        currentNode = openSet[i];
                    }
                }
                Profiler.EndSample();
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                    return RetracePath(startNode, targetNode);

                Profiler.BeginSample("Pathfind: Neighbour Search");
                foreach (Node neighbour in GetNeighbours(currentNode))
                {
                    if (neighbour.Cell.Blocked || closedSet.Contains(neighbour))
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
                j++;
                Profiler.EndSample();
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
