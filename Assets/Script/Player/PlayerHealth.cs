using System.Collections;
using System.Globalization;
using UnityEngine;
using Game.Core;
using TMPro;

public class PlayerHealth : MonoBehaviour, IHealthSystem
{
    [SerializeField] public Data playerData;
    private PlayerHealthBar playerHealthBar;
    private Transform petAI;
    [SerializeField] private GameObject uIDamagePlayer;
    private TextMeshProUGUI txtDamage;
    private GameObject uIDamageInstance;

    private void Awake()
    {
        playerHealthBar = FindObjectOfType<PlayerHealthBar>()?.GetComponent<PlayerHealthBar>();
        petAI = FindObjectOfType<PetAI>().transform;
        txtDamage = uIDamagePlayer.GetComponentInChildren<TextMeshProUGUI>();
        if (PlayerPrefs.GetFloat("currentHealth") == 0 || PlayerPrefs.GetFloat("maxHealth") == 0)
        {
            SetMaxHealth(this.playerData.heathDefault, this.playerData.hpIc);
        }
        else
        {
            GetCurrentHealth();
        }
    }

    private void SetMaxHealth(float maxHealth, float hpIc)
    {
        playerData.maxHealth = maxHealth + hpIc;
        playerData.currentHealth = this.playerData.maxHealth;
        playerHealthBar.SetHealth(this.playerData.currentHealth, this.playerData.maxHealth);
        PlayerPrefs.SetFloat("currentHealth", this.playerData.currentHealth);
        PlayerPrefs.SetFloat("maxHealth", this.playerData.maxHealth);
    }

    private void GetCurrentHealth()
    {
        playerData.maxHealth = PlayerPrefs.GetFloat("maxHealth");
        playerData.currentHealth = PlayerPrefs.GetFloat("currentHealth");
        playerHealthBar.SetHealth(PlayerPrefs.GetFloat("currentHealth"), PlayerPrefs.GetFloat("maxHealth"));
    }

    public void GetDamage(float damage)
    {
        playerData.currentHealth = Mathf.Clamp(playerData.currentHealth - damage, 0, playerData.maxHealth);
        if (playerData.currentHealth <= 0) Die();
        this.txtDamage.text = damage.ToString(CultureInfo.CurrentCulture);
        playerHealthBar.SetHealth(playerData.currentHealth, playerData.maxHealth);
        uIDamageInstance = Instantiate(this.uIDamagePlayer, transform.position + Vector3.up, Quaternion.identity);
        StartCoroutine(nameof(DestroyDamageFlying), 0.5f);
        PlayerPrefs.SetFloat("currentHealth", playerData.currentHealth);
        PlayerPrefs.SetFloat("maxHealth", playerData.maxHealth);
    }

    private IEnumerator DestroyDamageFlying(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(uIDamageInstance);
    }

    public void Heal(float value)
    {
        playerData.currentHealth = Mathf.Clamp(playerData.currentHealth + value, 0f, playerData.maxHealth);
        if (playerData.currentHealth > playerData.maxHealth)
            playerData.currentHealth = playerData.maxHealth;
        playerHealthBar.SetHealth(playerData.currentHealth, playerData.maxHealth);
        PlayerPrefs.SetFloat("currentHealth", playerData.currentHealth);
        PlayerPrefs.SetFloat("maxHealth", playerData.maxHealth);
    }

    public void Die()
    {
        playerData.currentHealth = 0f;
        StartCoroutine(nameof(TimeDelayDeath), 3f);
    }

    public bool PlayerIsDeath()
    {
        return playerData.currentHealth <= 0f;
    }

    private IEnumerator TimeDelayDeath(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetMaxHealth(playerData.heathDefault, playerData.hpIc);
        var transform1 = transform;
        transform1.position = playerData.position;
        petAI.transform.position = transform1.up;
        //transform.position = new Vector3(-4.95f, -4f, 0f);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
}

