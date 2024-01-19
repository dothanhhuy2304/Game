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
    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.instance;
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
                    UserName = userName.text,
                    CreatedDate = DateTime.UtcNow,
                    Age = int.Parse(age.text),
                    Gender = true,
                    DeviceId = SystemInfo.deviceUniqueIdentifier,
                    DeviceName = SystemInfo.deviceName,
                    Os = SystemInfo.operatingSystem,
                    AppVersion = Application.version,
                    OsVer = Environment.OSVersion.Version.ToString(),
                    Status = 0,
                    AvatarId = 0
                };

                //DataService.GetConnection().Table<DataService.PlayerProfileData>().Connection.InsertOrReplace(playerProfileData);
                HuyManager.Instance.UpdatePlayerProfile(playerProfileData);
                
                HuyManager.Instance.userId = playerProfileData.Id;
                DataService.GameData gameData = new DataService.GameData
                {
                    PlayerId = HuyManager.Instance.userId,
                    CharacterSelect = 0,
                    LevelId = 0,
                    Gold = 0,
                    Diamond = 0,
                    PositionX = -4.95f,
                    PositionY = -4f,
                    PositionZ = 0f,
                    Health = 100
                };
                //DataService.GetConnection().Table<DataService.GameData>().Connection.InsertOrReplace(gameData);
                HuyManager.Instance.UpdateUserData(gameData);
                
                DataService.PlayerSetting playerSetting = new DataService.PlayerSetting
                {
                    PlayerId = HuyManager.Instance.userId,
                    SoundMusic = 1f,
                    SoundEffect = 1f
                };
                //DataService.GetConnection().Table<DataService.PlayerSetting>().Connection.InsertOrReplace(playerSetting);
                HuyManager.Instance.UpdatePlayerSetting(playerSetting);
                
                DataService.Item item = new DataService.Item {Name = "tEst", Value = 2};
                DataService.GetConnection().Table<DataService.Item>().Connection.Insert(item);
                //Load Data
                HuyManager.Instance.currentPosition[0] = gameData.PositionX;
                HuyManager.Instance.currentPosition[1] = gameData.PositionY;
                HuyManager.Instance.currentPosition[2] = gameData.PositionZ;
                var data = HuyManager.GetCurrentPlayerData();
                _gameManager.SetScore(data.Score);
                _gameManager.SetMoney(data.Gold);
                _gameManager.SetDiamond(data.Diamond);
                animator.Play("Down");
            }).AppendInterval(0.6f)
            .AppendCallback(() =>
            {
                Destroy(gameObject);
            }).Play();
    }
}
