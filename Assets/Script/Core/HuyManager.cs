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

    public static void SetPlayerIsHurt(int state)
    {
        PlayerPrefs.SetInt("PlayerHurt", state);
    }

    public static bool GetPlayerIsHurt()
    {
        return PlayerPrefs.GetInt("PlayerHurt") == 1;
    }

    public static void SetTimeAttack(ref float currentTime)
    {
        if (currentTime > 0f)
        {
            currentTime -= Time.deltaTime;
        }
        else
        {
            currentTime = 0f;
        }
    }
}
