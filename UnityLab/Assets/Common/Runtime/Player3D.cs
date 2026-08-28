using UnityEngine;

namespace Kyrnel
{
    public class Player3D : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 2f;
        [SerializeField] private float _gravityScale = 1f; // 重力倍数（默认1）

        private Rigidbody _rb;
        private float _yaw;

        private void Start()
        {
            // 获取 Rigidbody 组件（若没有则自动添加）
            _rb = GetComponent<Rigidbody>();
            if (_rb == null)
                _rb = gameObject.AddComponent<Rigidbody>();

            // 冻结旋转的 X 和 Z 轴，防止物理倾倒；Y 轴由我们手动控制
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            // 建议使用连续检测，避免高速穿透
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            // 锁定鼠标（提升体验）
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // 鼠标水平旋转（仅在 Update 中处理输入）
            float mouseX = Input.GetAxis("Mouse X");
            _yaw += mouseX * _rotationSpeed;
            // 直接设置旋转（仅 Y 轴）
            transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
        }

        private void FixedUpdate()
        {
            // 获取 WASD 输入
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // 计算移动方向（基于角色的前向和右向）
            Vector3 moveDirection = (transform.right * horizontal + transform.forward * vertical).normalized;

            // 设置水平速度（保持原有 Y 轴速度，用于重力）
            Vector3 velocity = _rb.velocity;
            velocity.x = moveDirection.x * _moveSpeed;
            velocity.z = moveDirection.z * _moveSpeed;

            // 应用重力（Physics.gravity 默认向下，乘以重力倍数可调整）
            velocity.y += Physics.gravity.y * (_gravityScale - 1f) * Time.fixedDeltaTime;
            // 或者直接使用 _rb.AddForce 方式，但为了精确控制速度，用 velocity 赋值更直接

            _rb.velocity = velocity;
        }
    }
}