using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyrnel.LinkGame
{
    /// <summary>
    /// 节点视图工厂（组件）：负责实例化、定位、重排与销毁 Node 视图对象。
    /// 不包含游戏规则逻辑，仅负责与场景节点视图的交互。
    /// GameManager 通过本组件访问节点视图与布局。
    /// </summary>
    public class NodeSpawner : MonoBehaviour
    {
        [SerializeField] private Node _nodePrefab;
        [SerializeField] private Transform _nodeParent;
        [SerializeField] private NodeConfigSO _nodeConfigSo;

        [SerializeField] private float _nodeSize = 100f;
        [SerializeField] private float _spacing = 10f;
        [SerializeField] private float _xPadding;
        [SerializeField] private float _yPadding;
        [SerializeField] private float _spawnDuration = 0.3f;

        private Node[,] _nodeMap;
        private BoardLayout _layout;

        /// <summary>节点父容器（连线图形也挂载其下）。</summary>
        public Transform NodeParent => _nodeParent;

        /// <summary>棋盘布局计算器。</summary>
        public BoardLayout Layout => _layout;

        /// <summary>创建节点视图容器并初始化布局。</summary>
        public void Initialize(int width, int height)
        {
            _layout = new BoardLayout(_nodeSize, _spacing, _xPadding, _yPadding);
            // 传入棋盘行列，使整盘以容器锚点 (0,0) 为中心对称展开（容器锚点需指向屏幕中心）
            _layout.Initialize(width, height);
            _nodeMap = new Node[width, height];
        }

        /// <summary>实例化一个节点并初始化，返回该节点。</summary>
        public Node SpawnNode(Action<Node> onClickAction, NodeType type, int x, int y)
        {
            if (_nodeMap == null)
            {
                Debug.LogError("[NodeSpawner] 未初始化棋盘，请先调用 Initialize。");
                return null;
            }

            Node node = Instantiate(_nodePrefab, _nodeParent);
            node.Position = new Vector2Int(x, y);
            _nodeMap[x, y] = node;

            node.GetComponent<RectTransform>().anchoredPosition = _layout.GetCellBottomLeft(x, y);

            Sprite sprite = _nodeConfigSo.GetSprite(type);
            node.Initialize(onClickAction, type, sprite);
            node.gameObject.name = $"Node_{x}_{y}_{type}";

            // 创建后立即播放生成动画
            node.PlaySpawnAnimation(_spawnDuration, 0f);

            return node;
        }

        /// <summary>按坐标获取节点视图，越界或已清除返回 null。</summary>
        public Node GetNode(int x, int y)
        {
            if (_nodeMap == null || x < 0 || x >= _nodeMap.GetLength(0) || y < 0 || y >= _nodeMap.GetLength(1))
            {
                return null;
            }

            return _nodeMap[x, y];
        }

        /// <summary>
        /// 按棋盘补位移动记录播放滑动动画：将节点平滑移动到目标格子，并更新节点坐标与映射。
        /// 所有移动并行播放，全部完成后回调 <paramref name="onComplete"/>。
        /// 可直接被外部协程 yield/StartCoroutine 等待全部播放完成。
        /// </summary>
        public IEnumerator ApplyMoves(List<BoardMove> moves, float duration, Action onComplete)
        {
            if (moves == null || moves.Count == 0)
            {
                onComplete?.Invoke();
                yield break;
            }

            // 预计算阶段：先快照所有参与移动的节点引用、目标坐标与目标位置，
            // 避免交叉移动（如 A→B、B→A）时逐条更新 _nodeMap 相互覆盖导致节点丢失。
            int validCount = 0;
            Node[] movingNodes = new Node[moves.Count];
            Vector2Int[] targetCells = new Vector2Int[moves.Count];
            Vector2[] targetPositions = new Vector2[moves.Count];

            for (int i = 0; i < moves.Count; i++)
            {
                BoardMove move = moves[i];
                Node node = GetNode(move.From.x, move.From.y);
                if (node == null)
                {
                    continue;
                }

                movingNodes[validCount] = node;
                targetCells[validCount] = move.To;
                targetPositions[validCount] = _layout.GetCellCenter(move.To.x, move.To.y);
                validCount++;
            }

            if (validCount == 0)
            {
                onComplete?.Invoke();
                yield break;
            }

            // 先统一清空所有旧位置映射（利用快照中的节点当前位置），再写入新位置映射，避免交叉覆盖
            for (int i = 0; i < validCount; i++)
            {
                Vector2Int from = movingNodes[i].Position;
                _nodeMap[from.x, from.y] = null;
            }

            for (int i = 0; i < validCount; i++)
            {
                Node node = movingNodes[i];
                Vector2Int to = targetCells[i];
                _nodeMap[to.x, to.y] = node;
                node.Position = to;
            }

            // 并行播放所有滑动动画
            int pending = validCount;
            for (int i = 0; i < validCount; i++)
            {
                Node node = movingNodes[i];
                node.PlayMoveAnimation(targetPositions[i], duration, () => pending--);
            }

            // 等待所有滑动动画播放完成
            while (pending > 0)
            {
                yield return null;
            }

            onComplete?.Invoke();
        }

        /// <summary>将棋盘模型中的最新类型同步到节点视图（用于重排后刷新贴图）。</summary>
        public void SyncBoardToView(LinkBoard board)
        {
            if (_nodeMap == null || board == null)
            {
                return;
            }

            for (int x = 0; x < _nodeMap.GetLength(0); x++)
            {
                for (int y = 0; y < _nodeMap.GetLength(1); y++)
                {
                    Node node = _nodeMap[x, y];
                    if (node == null)
                    {
                        continue;
                    }

                    NodeType type = board.GetTypeAt(new Vector2Int(x, y));
                    if (node.Type != type)
                    {
                        node.ApplyType(type, _nodeConfigSo.GetSprite(type));
                    }
                }
            }
        }

        /// <summary>清除指定节点：从棋盘移除并销毁视图。</summary>
        public void ClearNode(Node node)
        {
            if (node == null)
            {
                return;
            }

            Vector2Int pos = node.Position;
            if (_nodeMap != null
                && pos.x >= 0 && pos.x < _nodeMap.GetLength(0)
                && pos.y >= 0 && pos.y < _nodeMap.GetLength(1)
                && _nodeMap[pos.x, pos.y] == node)
            {
                _nodeMap[pos.x, pos.y] = null;
            }

            // 先彻底隐藏再销毁，避免微信小游戏上 Destroy 延迟导致残影
            node.gameObject.SetActive(false);
            Destroy(node.gameObject);
        }

        /// <summary>清除整个棋盘的所有节点视图。</summary>
        public void ClearBoard()
        {
            if (_nodeMap == null)
            {
                return;
            }

            for (int x = 0; x < _nodeMap.GetLength(0); x++)
            {
                for (int y = 0; y < _nodeMap.GetLength(1); y++)
                {
                    Node node = _nodeMap[x, y];
                    if (node != null)
                    {
                        Destroy(node.gameObject);
                    }
                }
            }

            _nodeMap = null;
            _layout = null;
        }
    }
}