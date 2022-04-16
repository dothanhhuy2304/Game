using System.Globalization;
using UnityEngine;

namespace Game.GamePlay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] private TMPro.TextMeshProUGUI txtScore;
        [SerializeField] private TMPro.TextMeshProUGUI txtDiamond;
        [SerializeField] private TMPro.TextMeshProUGUI txtMoney;

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