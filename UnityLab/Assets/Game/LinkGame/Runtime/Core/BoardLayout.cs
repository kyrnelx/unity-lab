using UnityEngine;

namespace Kyrnel.LinkGame
{
    /// <summary>
    /// 棋盘布局计算：根据节点尺寸、间距与边距计算每个格子的锚点位置。
    /// 调用 <see cref="Initialize"/> 传入棋盘行列后，整盘会以容器锚点 (0,0)
    /// 为中心向四周对称展开，棋盘尺寸变化时始终居中，不再偏向角落。
    /// </summary>
    public sealed class BoardLayout
    {
        private readonly float _nodeSize;
        private readonly float _spacing;
        private readonly float _xPadding;
        private readonly float _yPadding;

        private float _offsetX;
        private float _offsetY;
        private bool _initialized;

        public BoardLayout(float nodeSize, float spacing, float xPadding, float yPadding)
        {
            _nodeSize = nodeSize;
            _spacing = spacing;
            _xPadding = xPadding;
            _yPadding = yPadding;
        }

        /// <summary>单个格子的步长（节点尺寸 + 间距）。</summary>
        public float Step => _nodeSize + _spacing;

        /// <summary>
        /// 传入棋盘行列数，计算使整盘以 (0,0) 为中心的偏移量。
        /// </summary>
        public void Initialize(int columns, int rows)
        {
            // 棋盘总尺寸 = 步长 × (格子数-1) + 节点尺寸
            float boardWidth = Step * (columns - 1) + _nodeSize;
            float boardHeight = Step * (rows - 1) + _nodeSize;

            // 将整盘平移到以 (0,0) 为中心：首格 = -boardWidth/2，末格 = +boardWidth/2
            // pivot=0.5 时 anchoredPosition 即节点中心（格线）；棋盘最外缘为 offset ± nodeSize/2。
            // 外缘中点 = offset + Step*(cols-1)/2，令其为 0 即居中：offset = -Step*(cols-1)/2
            // （nodeSize/2 在两端相互抵消，不参与居中计算）
            _offsetX = -Step * (columns - 1) * 0.5f;
            _offsetY = -Step * (rows - 1) * 0.5f;
            _initialized = true;
        }

        /// <summary>格子左下角锚点（节点 Pivot 在中心时的 anchoredPosition）。</summary>
        public Vector2 GetCellBottomLeft(int x, int y)
        {
            if (_initialized)
            {
                return new Vector2(_offsetX + Step * x, _offsetY + Step * y);
            }

            // 未初始化的旧行为：从 padding 起算（兼容性保留）
            return new Vector2(_xPadding + Step * x, _yPadding + Step * y);
        }

        /// <summary>
        /// 格子中心点。节点 pivot=0.5 时 anchoredPosition 即中心，
        /// 因此与 GetCellBottomLeft 返回同一位置。
        /// </summary>
        public Vector2 GetCellCenter(int x, int y)
        {
            return GetCellBottomLeft(x, y);
        }
    }
}