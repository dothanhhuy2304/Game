using UnityEngine;

namespace Game.GamePlay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private ScoreData scoreData;
        [SerializeField] private TMPro.TextMeshProUGUI txtScore;
        [SerializeField] private TMPro.TextMeshProUGUI txtDiamond;
        [SerializeField] private TMPro.TextMeshProUGUI txtMoney;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(this);
            scoreData.currentScore = 0f;
            SetScore(0f);
            SetMoney(0f);
            SetDiamond(0f);
        }

        public void SetScore(float score)
        {
            scoreData.currentScore += score;
            txtScore.text = scoreData.currentScore.ToString(System.Globalization.CultureInfo.CurrentCulture);
        }

        public void SetDiamond(float diamond)
        {
            scoreData.diamond += diamond;
            txtDiamond.text = scoreData.diamond.ToString(System.Globalization.CultureInfo.CurrentCulture);
        }

        public void SetMoney(float money)
        {
            scoreData.money += money;
            txtMoney.text = scoreData.money + " $";
        }

    }
}