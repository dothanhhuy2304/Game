using System;
using System.Linq;
using DG.Tweening;
using Script.Player;
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
        
        public string userId;
        public int characterSelected;
        public int currentScreen;
        public int saveScreenPass;
        public float[] currentPosition = new float[3];

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

        public void CameraShake(Camera cam, float duration, Vector3 strength, int vibrato, float randomness,
            bool fadeOut)
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

        public DataService.GameData GetCurrentPlayerData()
        {
            return DataService.GetConnection().Table<DataService.GameData>().FirstOrDefault();
        }

        public DataService.PlayerProfileData GetCurrentPlayerProfile()
        {
            return DataService.GetConnection().Table<DataService.PlayerProfileData>().FirstOrDefault();
        }

        public void UpdateUserData(DataService.GameData pf)
        {
            DataService.GameData gameData = new DataService.GameData()
            {
                PlayerId = pf.PlayerId,
                characterSelect = pf.characterSelect,
                levelId = pf.levelId,
                positionX = pf.positionX,
                positionY = pf.positionY,
                positionZ = pf.positionZ,
                score = pf.score,
                gold = pf.gold,
                diamond = pf.diamond,
                health = pf.health
            };
            DataService.GetConnection().Table<DataService.GameData>().Connection.InsertOrReplace(gameData);
        }

        public void UpdatePlayerProfile(DataService.PlayerProfileData profileData)
        {
            DataService.PlayerProfileData playerProfileData = new DataService.PlayerProfileData
            {
                Id = profileData.Id,
                userName = profileData.userName,
                createdDate = profileData.createdDate,
                age = profileData.age,
                gender = profileData.gender,
                deviceId = profileData.deviceId,
                deviceName = profileData.deviceName,
                os = profileData.os,
                appVersion = profileData.appVersion,
                osVer = profileData.osVer,
                status = profileData.status,
                avatarId = profileData.avatarId
            };
            DataService.GetConnection().Table<DataService.PlayerProfileData>().Connection.InsertOrReplace(playerProfileData);
        }

        public void UpdatePlayerSetting(DataService.PlayerSetting setting)
        {
            DataService.PlayerSetting playerSetting=new DataService.PlayerSetting();
            playerSetting.PlayerId = userId;
            playerSetting.soundMusic = setting.soundMusic;
            playerSetting.soundEffect = setting.soundEffect;
            DataService.GetConnection().Table<DataService.PlayerSetting>().Connection.InsertOrReplace(playerSetting);
        }

        public void ChangeSettingSoundMusic(float value)
        {
            if (DataService.GetConnection().Table<DataService.PlayerSetting>().Any())
            {
                DataService.GetConnection().Execute($"update PlayerSetting set soundMusic = '{value}' where PlayerId = '{userId}'");
            }
            else
            {
                Debug.Log("player setting is null");
            }
        }

        public void ChangeSettingSoundEffect(float value)
        {
            if (DataService.GetConnection().Table<DataService.PlayerSetting>().Any())
            {
                DataService.GetConnection().Execute($"update PlayerSetting set soundEffect = '{value}' where PlayerId = '{userId}'");
            }
            else
            {
                Debug.Log("player setting is null");
            }
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