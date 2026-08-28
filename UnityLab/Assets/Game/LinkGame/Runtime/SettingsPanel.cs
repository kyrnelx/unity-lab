using UnityEngine;
using UnityEngine.UI;

namespace Kyrnel.LinkGame
{
    /// <summary>
    /// 设置面板：控制音效音量大小，以及面板的打开/关闭。
    /// </summary>
    public class SettingsPanel : MonoBehaviour
    {
        [Header("面板")]
        [SerializeField] private GameObject _panel;

        [Header("音量")]
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private AudioSource _musicSource;

        [Header("按钮")]
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private AudioClip _buttonClickSound;


        private const string VolumePrefsKey = "LinkGame_Volume";
        private const string MusicPrefsKey = "LinkGame_Music";

        private void Awake()
        {
            // 读取上次保存的音效音量
            float savedVolume = PlayerPrefs.GetFloat(VolumePrefsKey, 1f);
            ApplyVolume(savedVolume);

            if (_volumeSlider != null)
            {
                _volumeSlider.value = savedVolume;
                _volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            }

            // 读取上次保存的音乐音量
            float savedMusic = PlayerPrefs.GetFloat(MusicPrefsKey, 1f);
            ApplyMusicVolume(savedMusic);

            if (_musicSlider != null)
            {
                _musicSlider.value = savedMusic;
                _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }


            if (_openButton != null)
            {
                _openButton.onClick.AddListener(Open);
            }

            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(Close);
            }

            // 默认关闭面板（启动时不播放按钮音效）
            if (_panel != null)
            {
                _panel.SetActive(false);
            }

        }

        private void OnVolumeChanged(float value)
        {
            ApplyVolume(value);
            PlayerPrefs.SetFloat(VolumePrefsKey, value);
            PlayerPrefs.Save();
        }

        private void ApplyVolume(float value)
        {
            if (_audioSource != null)
            {
                _audioSource.volume = Mathf.Clamp01(value);
            }
        }

        private void OnMusicVolumeChanged(float value)
        {
            ApplyMusicVolume(value);
            PlayerPrefs.SetFloat(MusicPrefsKey, value);
            PlayerPrefs.Save();
        }

        private void ApplyMusicVolume(float value)
        {
            if (_musicSource != null)
            {
                _musicSource.volume = Mathf.Clamp01(value);
            }
        }

        public void Open()
        {
            PlayButtonClickSound();
            if (_panel != null)
            {
                _panel.SetActive(true);
            }
        }

        public void Close()
        {
            PlayButtonClickSound();
            if (_panel != null)
            {
                _panel.SetActive(false);
            }
        }

        public void Toggle()
        {
            if (_panel != null)
            {
                _panel.SetActive(!_panel.activeSelf);
            }
        }

        /// <summary>播放按钮点击音效。</summary>
        private void PlayButtonClickSound()
        {
            if (_buttonClickSound == null || _audioSource == null)
            {
                return;
            }

            _audioSource.PlayOneShot(_buttonClickSound);
        }

    }
}
