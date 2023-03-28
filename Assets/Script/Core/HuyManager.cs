using System;
using DG.Tweening;
using UnityEngine;

namespace Script.Core
{
    public static class HuyManager
    {
        public static Action eventResetWhenPlayerDeath;

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

        public static void CameraShake(Camera camera, float duration, Vector3 strength, int vibrato, float randomness,
            bool fadeOut)
        {
            camera.DOShakePosition(duration, strength, vibrato, randomness, fadeOut);
        }
    }

    public enum EnemyType
    {
        Player,
        Ninja,
        CarnivorousPlant,
        Pet,
        Bee,
        Trunk,
    }
}