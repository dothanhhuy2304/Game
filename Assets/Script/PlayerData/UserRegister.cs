using System;
using DG.Tweening;
using Script.Core;
using Script.GamePlay;
using TMPro;
using UnityEngine;

public class UserRegister : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_InputField userName;
    [SerializeField] private TMP_InputField age;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void OnEnable()
    {
        animator.Play("Up");
    }

    public void SendData()
    {
        if (string.IsNullOrWhiteSpace(userName.text) || string.IsNullOrWhiteSpace(age.text))
        {
            return;
        }

        DOTween.Sequence()
            .AppendCallback(() =>
            {
                DataService.PlayerProfileData playerProfileData = new DataService.PlayerProfileData
                {
                    Id = SystemInfo.deviceUniqueIdentifier,
                    userName = userName.text,
                    createdDate = DateTime.UtcNow,
                    age = int.Parse(age.text),
                    gender = true,
                    deviceId = SystemInfo.deviceUniqueIdentifier,
                    deviceName = SystemInfo.deviceName,
                    os = SystemInfo.operatingSystem,
                    appVersion = Application.version,
                    osVer = Environment.OSVersion.Version.ToString(),
                    status = 0,
                    avatarId = 0
                };

                //DataService.GetConnection().Table<DataService.PlayerProfileData>().Connection.InsertOrReplace(playerProfileData);
                HuyManager.Instance.UpdatePlayerProfile(playerProfileData);
                
                HuyManager.Instance.userId = playerProfileData.Id;
                DataService.GameData gameData = new DataService.GameData
                {
                    PlayerId = HuyManager.Instance.userId,
                    characterSelect = 0,
                    levelId = 0,
                    gold = 0,
                    diamond = 0,
                    positionX = -4.95f,
                    positionY = -4f,
                    positionZ = 0f,
                    health = 100
                };
                //DataService.GetConnection().Table<DataService.GameData>().Connection.InsertOrReplace(gameData);
                HuyManager.Instance.UpdateUserData(gameData);
                
                DataService.PlayerSetting playerSetting = new DataService.PlayerSetting
                {
                    PlayerId = HuyManager.Instance.userId,
                    soundMusic = 1f,
                    soundEffect = 1f
                };
                //DataService.GetConnection().Table<DataService.PlayerSetting>().Connection.InsertOrReplace(playerSetting);
                HuyManager.Instance.UpdatePlayerSetting(playerSetting);
                
                DataService.Item item = new DataService.Item {name = "tEst", value = 2};
                DataService.GetConnection().Table<DataService.Item>().Connection.Insert(item);
                //Load Data
                HuyManager.Instance.currentPosition[0] = gameData.positionX;
                HuyManager.Instance.currentPosition[1] = gameData.positionY;
                HuyManager.Instance.currentPosition[2] = gameData.positionZ;
                var data = HuyManager.Instance.GetCurrentPlayerData();
                gameManager.SetScore(data.score);
                gameManager.SetMoney(data.gold);
                gameManager.SetDiamond(data.diamond);
                animator.Play("Down");
            }).AppendInterval(0.6f)
            .AppendCallback(() =>
            {
                Destroy(gameObject);
            }).Play();
    }
}
