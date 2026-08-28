using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Kyrnel.LinkGame
{
    public class Node : MonoBehaviour
    {
        public Vector2Int Position { get; set; }
        public NodeType Type { get; set; }

        [SerializeField] private Image _image;
        [SerializeField] private Button _button;

        private Action<Node> _onClickAction;
        private Color _normalColor = Color.white;
        private Color _selectedColor = new Color(0.75f, 0.9f, 1f, 1f);

        public void Initialize(Action<Node> onClickAction, NodeType nodeType, Sprite nodeSprite)
        {
            _onClickAction = onClickAction;
            _button.onClick.AddListener(OnClick);
            Type = nodeType;
            _image.sprite = nodeSprite;
            _image.color = _normalColor;
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OnClick()
        {
            _onClickAction?.Invoke(this);
        }

        /// <summary>设置选中状态，通过改变图片颜色提供视觉反馈。</summary>
        public void SetSelected(bool selected)
        {
            _image.color = selected ? _selectedColor : _normalColor;
        }

        /// <summary>应用新的类型与贴图（用于重排）。</summary>
        public void ApplyType(NodeType type, Sprite sprite)
        {
            Type = type;
            _image.sprite = sprite;
        }

        /// <summary>生成动画：从 0 缩放弹出，可带延迟用于错峰。</summary>
        public void PlaySpawnAnimation(float duration, float delay)
        {
            StartCoroutine(SpawnRoutine(duration, delay));
        }

        private IEnumerator SpawnRoutine(float duration, float delay)
        {
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            transform.localScale = Vector3.zero;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float p = Mathf.Clamp01(t / duration);
                transform.localScale = Vector3.one * EaseOutBack(p);
                yield return null;
            }

            transform.localScale = Vector3.one;
        }

        /// <summary>滑动补位动画：从当前位置平滑移动到目标单元格位置。</summary>
        public void PlayMoveAnimation(Vector2 destination, float duration, Action onComplete)
        {
            StartCoroutine(MoveRoutine(destination, duration, onComplete));
        }

        private IEnumerator MoveRoutine(Vector2 destination, float duration, Action onComplete)
        {
            RectTransform rect = transform as RectTransform;
            Vector2 start = rect != null ? rect.anchoredPosition : Vector2.zero;

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float p = Mathf.Clamp01(t / duration);
                float eased = 1f - Mathf.Pow(1f - p, 3f); // EaseOutCubic

                if (rect != null)
                {
                    rect.anchoredPosition = Vector2.Lerp(start, destination, eased);
                }

                yield return null;
            }

            if (rect != null)
            {
                rect.anchoredPosition = destination;
            }

            onComplete?.Invoke();
        }

        /// <summary>消除动画：缩小消失，完成后回调。</summary>
        public void PlayClearAnimation(float duration, Action onComplete)
        {
            StartCoroutine(ClearRoutine(duration, onComplete));
        }

        private IEnumerator ClearRoutine(float duration, Action onComplete)
        {
            float t = 0f;
            Vector3 startScale = transform.localScale;
            while (t < duration)
            {
                t += Time.deltaTime;
                float p = Mathf.Clamp01(t / duration);
                transform.localScale = startScale * (1f - EaseInCubic(p));
                yield return null;
            }

            transform.localScale = Vector3.zero;

            // 彻底隐藏，避免微信小游戏上 Destroy 延迟导致残影
            if (_image != null)
            {
                _image.enabled = false;
            }

            if (_button != null)
            {
                _button.enabled = false;
            }

            // 先回调再禁用 GameObject，避免协程被停止导致回调丢失
            onComplete?.Invoke();

            // 禁用整个 GameObject，彻底阻止 Canvas 延迟重建把旧画面重新画出来
            gameObject.SetActive(false);
        }




        private static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }

        private static float EaseInCubic(float t)
        {
            return t * t * t;
        }
    }
}
