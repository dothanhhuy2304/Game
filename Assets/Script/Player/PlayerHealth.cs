using System.Collections;
using System.Globalization;
using UnityEngine;
using Game.Core;
using Game.Player;

public class PlayerHealth : MonoBehaviour, IHealthSystem
{
    [SerializeField] public Data playerData;
    [SerializeField] public PlayerData playerDatas;
    [SerializeField] private CharacterController2D player;
    private PlayerHealthBar playerHealthBar;
    private Transform petAI;
    [SerializeField] private GameObject uIDamagePlayer;
    private TMPro.TextMeshProUGUI txtDamage;

    private void Start()
    {
        player = FindObjectOfType<CharacterController2D>().GetComponent<CharacterController2D>();
        playerHealthBar = FindObjectOfType<PlayerHealthBar>()?.GetComponent<PlayerHealthBar>();
        petAI = FindObjectOfType<PetAI>()?.transform;
        txtDamage = uIDamagePlayer.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (playerData.currentHealth == 0)
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
    }

    private void GetCurrentHealth()
    {
        playerHealthBar.SetHealth(this.playerData.currentHealth, this.playerData.maxHealth);
    }

    public void GetDamage(float damage)
    {
        playerData.currentHealth = Mathf.Clamp(playerData.currentHealth - damage, 0, playerData.maxHealth);
        player.PlayerHurt();
        if (playerData.currentHealth <= 0) Die();
        this.txtDamage.text = damage.ToString(CultureInfo.CurrentCulture);
        playerHealthBar.SetHealth(this.playerData.currentHealth, this.playerData.maxHealth);
        var uIDamageInstance = Instantiate(this.uIDamagePlayer, transform.position + Vector3.up, Quaternion.identity);
        Destroy(uIDamageInstance, 0.5f);
    }

    public void Heal(float value)
    {
        playerData.currentHealth = Mathf.Clamp(playerData.currentHealth + value, 0f, playerData.maxHealth);
        if (playerData.currentHealth > playerData.maxHealth)
            this.playerData.currentHealth = this.playerData.maxHealth;
        playerHealthBar.SetHealth(this.playerData.currentHealth, this.playerData.maxHealth);
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
        SetMaxHealth(playerData.heathDefault, playerData.hpIc);
        var transform1 = transform;
        transform1.position = playerDatas.position;
        petAI.transform.position = transform1.up;
        //transform.position = new Vector3(-4.95f, -4f, 0f);
    }

    private void OnApplicationQuit()
    {
        playerData.currentHealth = 0f;
    }
}

