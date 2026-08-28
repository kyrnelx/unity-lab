using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyrnel
{
    public class BfsPathfinding : MonoBehaviour
    {
        public int mapWidth;
        public int mapHeight;

        public List<Vector2Int> obstacles;

        public PathfindingNode nodePrefab;
        public Transform nodeParent;

        private bool[,] _map;
        private PathfindingNode[,] _nodeMap;

        private Queue<PathfindingNode> _searchQueue = new Queue<PathfindingNode>();
        private HashSet<PathfindingNode> _visited = new HashSet<PathfindingNode>();

        private List<PathfindingNode> _neighborsBuffer = new List<PathfindingNode>();

        private List<Vector2Int> _directions = new List<Vector2Int>()
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
        };

        private void Awake()
        {
            _map = new bool[mapWidth, mapHeight];
            _nodeMap = new PathfindingNode[mapWidth, mapHeight];

            for (int i = 0; i < mapHeight; i++) // 从左下角（lower left）开始，按行从左向右平铺，当前行铺满后向上移动一行。
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    PathfindingNode node = Instantiate(nodePrefab, Vector3.zero, Quaternion.identity, nodeParent);
                    Vector2Int nodePosition = new Vector2Int(j, i);
                    bool isWalkable = !obstacles.Contains(new Vector2Int(j, i));
                    node.SetData(nodePosition, isWalkable);
                    _map[j, i] = isWalkable;
                    _nodeMap[j, i] = node;
                }
            }

            StartCoroutine(FindPathCoroutine());
        }

        private IEnumerator FindPathCoroutine()
        {
            var startNode = _nodeMap[2, 6];
            var endNode = _nodeMap[6, 2];

            if (!startNode.IsWalkable || !endNode.IsWalkable ||
                !IsInsideMap(startNode.Position) || !IsInsideMap(endNode.Position))
            {
                Debug.LogError("无效起点和终点。");
                yield break;
            }

            startNode.MarkStart();
            _searchQueue.Enqueue(startNode);
            _visited.Add(startNode);

            while (_searchQueue.Count > 0)
            {
                PathfindingNode node = _searchQueue.Dequeue();
                node.MarkWalked();
                if (node.Position == endNode.Position)
                {
                    RetracePath(endNode, startNode);
                    yield break;
                }

                RefreshNeighbors(node);
                foreach (var neighbor in _neighborsBuffer)
                {
                    if (!_visited.Contains(neighbor))
                    {
                        _searchQueue.Enqueue(neighbor);
                        _visited.Add(neighbor);
                        neighbor.MarkVisited();
                        neighbor.Parent = node;

                        if (neighbor.Position == endNode.Position)
                        {
                            RetracePath(endNode, startNode);
                            yield break;
                        }

                        yield return new WaitForSeconds(0.25f);
                    }
                }
            }
        }

        private void RetracePath(PathfindingNode endNode, PathfindingNode startNode)
        {
            PathfindingNode node = endNode;
            node.MarkPath();
            while (node.Position != startNode.Position)
            {
                node = node.Parent;
                node.MarkPath();
            }
        }

        private void RefreshNeighbors(PathfindingNode node)
        {
            _neighborsBuffer.Clear();
            foreach (var direction in _directions)
            {
                Vector2Int neighborPos = node.Position + direction;
                if (IsInsideMap(neighborPos) &&
                    !_visited.Contains(_nodeMap[neighborPos.x, neighborPos.y]) &&
                    _nodeMap[neighborPos.x, neighborPos.y].IsWalkable)
                {
                    _neighborsBuffer.Add(_nodeMap[neighborPos.x, neighborPos.y]);
                }
            }
        }

        private bool IsInsideMap(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < mapWidth && pos.y >= 0 && pos.y < mapHeight;
        }
    }
}