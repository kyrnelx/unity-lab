using UnityEngine;
using UnityEngine.UI;

namespace Kyrnel.LinkGame
{
    /// <summary>
    /// HUD 控制器：负责刷新积分、关卡与累计总分文本。
    /// 从 GameManager 拆分出的单一职责类。
    /// </summary>
    public class GameHudController : MonoBehaviour
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _levelText;
        [SerializeField] private Text _totalScoreText;

        public void SetScore(int score)
        {
            if (_scoreText != null)
            {
                _scoreText.text = score.ToString();
            }
        }

        public void SetLevel(int level)
        {
            if (_levelText != null)
            {
                _levelText.text = $"第 {level} 关";
            }
        }

        public void SetTotalScore(int totalScore)
        {
            if (_totalScoreText != null)
            {
                _totalScoreText.text = totalScore.ToString();
            }
        }
    }
}