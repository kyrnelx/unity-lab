using UnityEngine;

namespace Kyrnel.LinkGame
{
    /// <summary>
    /// 游戏音频控制器：负责点击/消除音效与背景音乐的播放。
    /// 从 GameManager 拆分出的单一职责类。
    /// </summary>
    public class GameAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip _clickSound;
        [SerializeField] private AudioClip _matchSound;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _bgmClip;
        [SerializeField] private AudioSource _bgmSource;

        /// <summary>初始化并播放背景音乐（循环）。</summary>
        public void Initialize()
        {
            PlayBgm();
        }

        public void PlayClickSound()
        {
            if (_clickSound == null)
            {
                return;
            }

            EnsureSfxSource();
            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(_clickSound);
            }
        }

        public void PlayMatchSound()
        {
            if (_matchSound == null)
            {
                return;
            }

            EnsureSfxSource();
            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(_matchSound);
            }
        }

        /// <summary>播放背景音乐（循环），若引用变化则切换。</summary>
        public void PlayBgm()
        {
            if (_bgmClip == null)
            {
                return;
            }

            EnsureBgmSource();
            if (_bgmSource == null)
            {
                return;
            }

            if (_bgmSource.clip != _bgmClip)
            {
                _bgmSource.clip = _bgmClip;
            }

            _bgmSource.loop = true;
            _bgmSource.playOnAwake = false;
            if (!_bgmSource.isPlaying)
            {
                _bgmSource.Play();
            }
        }

        /// <summary>确保存在可用的音效 AudioSource，未指定时自动挂载。</summary>
        private void EnsureSfxSource()
        {
            if (_audioSource != null)
            {
                return;
            }

            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
            }
        }

        /// <summary>确保存在可用的背景音乐 AudioSource，未指定时自动挂载。</summary>
        private void EnsureBgmSource()
        {
            if (_bgmSource != null)
            {
                return;
            }

            _bgmSource = GetComponent<AudioSource>();
            if (_bgmSource == null)
            {
                _bgmSource = gameObject.AddComponent<AudioSource>();
                _bgmSource.playOnAwake = false;
            }
        }
    }
}