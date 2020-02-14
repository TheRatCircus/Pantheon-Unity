// Pathfinder.cs
// Courtesy of Sebastian Lague

#define DEBUG_PF
#undef DEBUG_PF

using Pantheon.Utils;
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
        private readonly Level level;
        private readonly Node[,] map;

        public Pathfinder(Level level)
        {
            this.level = level;
            map = new Node[level.Size.x, level.Size.y];
            for (int y = 0; y < level.Size.y; y++)
                for (int x = 0; x < level.Size.x; x++)
                    map[x, y] = new Node() { Position = new Vector2Int(x, y) };
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
                        if (map.TryGet(out Node n,
                            node.Position.x + x,
                            node.Position.y + y))
                            neighbours.Add(n);
                    }

            Profiler.EndSample();
            return neighbours;
        }

        public Line GetPath(Vector2Int startPos,
            Vector2Int targetPos)
        {
            Profiler.BeginSample("Pathfinding");
            List<Node> nodes = FindPath(startPos, targetPos);
            Profiler.EndSample();

            if (nodes == null)
                return new Line(0);

            Line cellPath = new Line(nodes.Count);

            foreach (Node n in nodes)
            {
                cellPath.Add(n.Position);
                DebugVisualize(n.Position);
            }

            return cellPath;
        }

        List<Node> FindPath(Vector2Int startPos, Vector2Int targetPos)
        {
            if (!map.TryGet(out Node startNode, startPos.x, startPos.y))
                throw new ArgumentException(
                    $"No node at {startPos}.");
            if (!map.TryGet(out Node targetNode, targetPos.x, targetPos.y))
                throw new ArgumentException(
                    $"No node at {targetPos}.");

            // TODO: Find adjacent free node?
            if (level.Walled(targetNode.Position))
                return null;

            Heap<Node> openSet = new Heap<Node>(map.Length);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            int j = 0;
            while (openSet.CurrentItemCount > 0)
            {
                if (j >= 10000)
                    throw new Exception(
                        $"Pathfinder iterated {j} times.");

                Profiler.BeginSample("Pathfind: Cost Evaluation");
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);
                Profiler.EndSample();

                if (currentNode == targetNode)
                    return RetracePath(startNode, targetNode);

                Profiler.BeginSample("Pathfind: Neighbour Search");
                foreach (Node neighbour in GetNeighbours(currentNode))
                {
                    if (level.Walled(neighbour.Position) || closedSet.Contains(neighbour))
                        continue;

                    DebugVisualize(neighbour.Position);

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

        [System.Diagnostics.Conditional("DEBUG_PF")]
        private void DebugVisualize(Vector2Int cell)
        {
            Debug.Visualisation.MarkPos(cell, Color.white, 10f);
        }
    }
}
