using System.Globalization;
using UnityEngine;

namespace Game.Player
{
    public class PlayerHealthBar : FastSingleton<PlayerHealthBar>
    {
        [SerializeField] private UnityEngine.UI.Image fill;
        [SerializeField] private TMPro.TextMeshProUGUI txtCurrentHealth;
        [SerializeField] private Material colorA, colorB;

        public void SetHealth(float currentHealth, float maxHealth)
        {
            txtCurrentHealth.text = currentHealth.ToString(CultureInfo.InvariantCulture);
            fill.fillAmount = currentHealth / maxHealth;
            fill.color = Color.Lerp(colorB.color, colorA.color, fill.fillAmount);
        }
    }
}