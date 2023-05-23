using System;
using DG.Tweening;
using UnityEngine;

namespace Script.Core
{
    public class HuyManager : Singleton<HuyManager>
    {
        public Action eventResetWhenPlayerDeath;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitOnLoad()
        {
            Init();
        }
        
        public void SetPlayerIsDeath(int state)
        {
            PlayerPrefs.SetInt("PlayerIsDeath", state);
        }

        public bool PlayerIsDeath()
        {
            return PlayerPrefs.GetInt("PlayerIsDeath") == 1;
        }

        public void SetPlayerIsHurt(int state)
        {
            PlayerPrefs.SetInt("PlayerHurt", state);
        }

        public bool GetPlayerIsHurt()
        {
            return PlayerPrefs.GetInt("PlayerHurt") == 1;
        }

        public void SetUpTime(ref float currentTime)
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

        public void CameraShake(Camera cam, float duration, Vector3 strength, int vibrato, float randomness, bool fadeOut)
        {
            cam.DOShakePosition(duration, strength, vibrato, randomness, fadeOut);
        }

        public Action RegisterEventPlayerDeath()
        {
            return eventResetWhenPlayerDeath;
        }

        public void EventPlayerDeath()
        {
            eventResetWhenPlayerDeath?.Invoke();
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