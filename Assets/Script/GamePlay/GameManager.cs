using System.Globalization;
using UnityEngine;
using TMPro;

namespace Game.GamePlay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private TextMeshProUGUI txtScore;
        [SerializeField] private TextMeshProUGUI txtDiamond;
        [SerializeField] private TextMeshProUGUI txtMoney;

        private void Awake()
        {
            DontDestroyOnLoad(this);

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetScore(float score)
        {
            txtScore.text = score.ToString(CultureInfo.CurrentCulture);
        }

        public void SetDiamond(float diamond)
        {
            txtDiamond.text = diamond.ToString(CultureInfo.CurrentCulture);
        }

        public void SetMoney(float money)
        {
            txtMoney.text = money.ToString(CultureInfo.CurrentCulture) + " $";
        }

    }
}