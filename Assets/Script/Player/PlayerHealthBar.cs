using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private TextMeshProUGUI txtCurrentHealth;
    [SerializeField] private Material colorA, colorB;

    public void SetHealth(float currentHealth, float maxHealth)
    {
        txtCurrentHealth.text = currentHealth.ToString(CultureInfo.InvariantCulture);
        fill.fillAmount = currentHealth / maxHealth;
        fill.color = Color.Lerp(colorB.color, colorA.color, fill.fillAmount);
    }
}