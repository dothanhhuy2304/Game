using System.Collections;
using UnityEngine;
using Game.Core;

public class PlayerHealth : MonoBehaviour, IHealthSystem
{
    [SerializeField] public Data playerData;
    private PlayerHealthBar playerHealthBar;

    private void Awake()
    {
        playerHealthBar = FindObjectOfType<PlayerHealthBar>()?.GetComponent<PlayerHealthBar>();
        if (PlayerPrefs.GetFloat("currentHealth") == 0 || PlayerPrefs.GetFloat("maxHealth") == 0)
        {
            SetMaxHealth(this.playerData.heathDefault, this.playerData.hpIc);
        }
        else
        {
            GetCurrentHealth();
        }
    }
    void SetMaxHealth(float maxHealth, float hpIc)
    {
        this.playerData.maxHealth = maxHealth + hpIc;
        this.playerData.currentHealth = this.playerData.maxHealth;
        playerHealthBar.SetHealth(this.playerData.currentHealth, this.playerData.maxHealth);
        PlayerPrefs.SetFloat("currentHealth", this.playerData.currentHealth);
        PlayerPrefs.SetFloat("maxHealth", this.playerData.maxHealth);
    }
    void GetCurrentHealth()
    {
        this.playerData.maxHealth = PlayerPrefs.GetFloat("maxHealth");
        this.playerData.currentHealth = PlayerPrefs.GetFloat("currentHealth");
        this.playerHealthBar.SetHealth(PlayerPrefs.GetFloat("currentHealth"), PlayerPrefs.GetFloat("maxHealth"));
    }
    public void GetDamage(float damage)
    {
        this.playerData.currentHealth = Mathf.Clamp(this.playerData.currentHealth - damage, 0, this.playerData.maxHealth);
        if (this.playerData.currentHealth <= 0) Die();
        playerHealthBar.SetHealth(this.playerData.currentHealth, this.playerData.maxHealth);
        PlayerPrefs.SetFloat("currentHealth", this.playerData.currentHealth);
        PlayerPrefs.SetFloat("maxHealth", this.playerData.maxHealth);

    }
    public void Heal(float value)
    {
        this.playerData.currentHealth = Mathf.Clamp(this.playerData.currentHealth + value, 0, this.playerData.maxHealth);
        if (this.playerData.currentHealth > this.playerData.maxHealth) this.playerData.currentHealth = this.playerData.maxHealth;
        playerHealthBar.SetHealth(this.playerData.currentHealth, this.playerData.maxHealth);
        PlayerPrefs.SetFloat("currentHealth", this.playerData.currentHealth);
        PlayerPrefs.SetFloat("maxHealth", this.playerData.maxHealth);

    }
    public void Die()
    {
        this.playerData.currentHealth = 0f;
        StartCoroutine(nameof(TimeDelayDeath), 3f);
    }
    public bool PlayerIsDeath()
    {
        return this.playerData.currentHealth <= 0f;
    }

    private IEnumerator TimeDelayDeath(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetMaxHealth(this.playerData.heathDefault, this.playerData.hpIc);
        transform.position = this.playerData.position;
        //transform.position = new Vector3(-4.95f, -4f, 0f);
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }

}

