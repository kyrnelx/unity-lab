using UnityEngine;

namespace Kyrnel
{
    public static class RectTransformExtensions
    {
        public static Vector2 GetGeometricCenter(this RectTransform rt)
        {
            // 步骤1：获取锚点定义的基准位置（世界坐标）
            Vector3 anchorBasedPosition = rt.position;
            // 步骤2：计算pivot偏移量（局部空间）
            Vector2 pivotOffset = new Vector2((0.5f - rt.pivot.x) * rt.rect.width, (0.5f - rt.pivot.y) * rt.rect.height);
            // 步骤3：将偏移量转换到世界空间
            Vector3 worldOffset = rt.TransformVector(pivotOffset); // 应用父级对象的旋转和缩放
            // 步骤4：计算真正的中心点
            return anchorBasedPosition + worldOffset;
        }
    }
}