using System.Globalization;
using UnityEngine;
using TMPro;

namespace Game.GamePlay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtScore;
        [SerializeField] private TextMeshProUGUI txtDiamond;
        [SerializeField] private TextMeshProUGUI txtMoney;

        private void Awake()
        {
            DontDestroyOnLoad(this);
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