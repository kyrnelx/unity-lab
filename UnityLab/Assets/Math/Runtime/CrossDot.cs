using UnityEngine;

namespace Kyrnel
{
    public class CrossDot : MonoBehaviour
    {
        public MonoBehaviour Enemy;

        public void Start()
        {
            // 获取从自身指向敌人的水平方向向量（忽略高度，仅考虑水平平面）
            Vector3 directionToEnemy = Enemy.transform.position - transform.position;
            directionToEnemy.y = 0f;

            // 阈值，用于判断方向是否“正对”某个轴向（避免浮点误差）
            const float eps = 0.01f;

            // 若水平距离极近，视为重合，直接返回（同时避免归一化零向量）
            float sqrDist = directionToEnemy.sqrMagnitude;
            if (sqrDist < eps * eps)
            {
                Debug.Log("敌人与自身重合");
                return; // 若后续还有逻辑，可改为 else 分支
            }

            // 归一化得到单位方向向量
            Vector3 normalizedDirection = directionToEnemy.normalized;

            // 点乘：结果 > 0 表示目标在自身前方，< 0 表示在后方
            float dot = Vector3.Dot(transform.forward, normalizedDirection);
            // 叉乘的 y 分量：结果 > 0 表示目标在自身右方，< 0 表示在左方
            float crossY = Vector3.Cross(transform.forward, normalizedDirection).y;

            // 使用阈值判断是否近似为零（避免浮点数微小偏差）
            bool isCrossZero = Mathf.Abs(crossY) < eps;
            bool isDotZero   = Mathf.Abs(dot)    < eps;

            if (isCrossZero && isDotZero)
            {
                // 理论不会触发（已排除重合），但保留作为安全兜底
                Debug.Log("敌人与自身重合");
            }
            else if (isCrossZero) // 叉乘为零 → 目标在正前方或正后方
            {
                Debug.Log(dot > 0 ? "正前方" : "正后方");
            }
            else if (isDotZero) // 点乘为零 → 目标在正右方或正左方
            {
                Debug.Log(crossY > 0 ? "正右方" : "正左方");
            }
            else if (dot > 0) // 前方区域（左前或右前）
            {
                Debug.Log(crossY > 0 ? "右前方" : "左前方");
            }
            else // 后方区域（左后或右后）
            {
                Debug.Log(crossY > 0 ? "右后方" : "左后方");
            }
        }
    }

    /*点乘 (Vector3.Dot) —— 用来判断【前后】
    dot > 0 ➔ 敌人在前方
    dot < 0 ➔ 敌人在后方
    dot == 0 ➔ 敌人在正两侧（垂直）
    叉乘 (Vector3.Cross) —— 用来判断【左右】
    cross.y > 0 ➔ 敌人在右边
    cross.y < 0 ➔ 敌人在左边
    cross.y == 0 ➔ 敌人正中线上（正前方或正后方）*/
}