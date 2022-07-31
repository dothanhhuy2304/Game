using System.Collections;
using Game.GamePlay;
using UnityEngine;

public static class HuyManager
{

    public static void SetPlayerIsDeath(int state)
    {
        PlayerPrefs.SetInt("PlayerIsDeath", state);
    }

    public static bool PlayerIsDeath()
    {
        return PlayerPrefs.GetInt("PlayerIsDeath") == 1;
    }

    public static IEnumerator EventDeath(Rigidbody2D body, Collider2D collider,Animator animator, float durationRespawn)
    {
        body.bodyType = RigidbodyType2D.Static;
        collider.enabled = false;
        animator.SetTrigger("is_Death");
        AudioManager.instance.Play("Enemy_Death");
        yield return new WaitForSeconds(durationRespawn);
        body.bodyType = RigidbodyType2D.Dynamic;
        collider.enabled = true;
    }

}
