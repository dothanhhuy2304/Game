using UnityEngine;

namespace Script.Player
{
    public class PlayerHealthBar : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image fill;
        [SerializeField] private Material colorA, colorB;
        [SerializeField] private GameObject canvas;

        public void SetHealth(float currentHealth, float maxHealth)
        {
            fill.fillAmount = currentHealth / maxHealth;
            fill.color = Color.Lerp(colorB.color, colorA.color, fill.fillAmount);
        }

        private void LateUpdate()
        {
            if (!canvas) return;
            canvas.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }
}