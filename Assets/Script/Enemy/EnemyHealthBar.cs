using UnityEngine;
using UnityEngine.UI;

namespace Script.Enemy
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Image fill;
        [SerializeField] private Material colorA, colorB;
        [SerializeField] private GameObject canvas;

        public void SetHealth(float currentHealth, float maxHealth)
        {
            fill.fillAmount = currentHealth / maxHealth;
            fill.color = Color.Lerp(colorB.color, colorA.color, fill.fillAmount);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            canvas.gameObject.SetActive(fill.fillAmount != currentHealth / maxHealth);
        }

        private void LateUpdate()
        {
            if (!canvas) return;
            canvas.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }
}