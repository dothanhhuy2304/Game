using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

namespace Game.GamePlay
{
    public class UserPref
    {
        public static string userId;
    }
    
    public class GameManager : FastSingleton<GameManager>
    {
        private const string ScoreData = "scoreData";
        private const string PlayerData = "playerData";
        [SerializeField] private UIManager uiManager;
        public CharacterScriptTableObject listCharacter;
        public PlayerData playerData;
        [SerializeField] private TextMeshProUGUI txtScore;
        [SerializeField] private TextMeshProUGUI txtDiamond;
        [SerializeField] private TextMeshProUGUI txtMoney;
        //
        private int numberScore;
        private int numberGold;
        private int numberDiamond;

        private void Start()
        {

            if (!DataService.GetConnection().Table<DataService.PlayerProfileData>().Any())
            {
                uiManager.PopupCreateAccount();
            }
            else
            {
                UserPref.userId = DataService.GetConnection().Table<DataService.PlayerProfileData>().FirstOrDefault().Id;
                Debug.LogError("Exit data");
            }

            if (LoadData<PlayerDataObj>(PlayerData) == null)
            {
                playerData.playerDataObj.position = new[] {-4.95f, -4f, 0};
                playerData.playerDataObj.characterSelection = 0;
                playerData.playerDataObj.currentScenes = 0;
                playerData.playerDataObj.saveAudio = false;
                playerData.playerDataObj.soundEffect = 0;
                playerData.playerDataObj.soundMusic = 0;
            }
            else
            {
                if (LoadData<PlayerDataObj>(PlayerData).position == null)
                {
                    playerData.playerDataObj.position = new[] {-4.95f, -4f, 0};
                }
                else
                {
                    playerData.playerDataObj.position = LoadData<PlayerDataObj>(PlayerData).position;
                }

                playerData.playerDataObj.characterSelection = LoadData<PlayerDataObj>(PlayerData).characterSelection;
                playerData.playerDataObj.currentScenes = LoadData<PlayerDataObj>(PlayerData).currentScenes;
                playerData.playerDataObj.saveAudio = LoadData<PlayerDataObj>(PlayerData).saveAudio;
                playerData.playerDataObj.soundEffect = LoadData<PlayerDataObj>(PlayerData).soundEffect;
                playerData.playerDataObj.soundMusic = LoadData<PlayerDataObj>(PlayerData).soundMusic;
            }

            var listPlayerData = DataService.GetConnection().Table<DataService.GameData>().ToList();
            foreach (var player in listPlayerData)
            {
                if (player.PlayerId.Equals(UserPref.userId))
                {
                    SetScore(player.score);
                    SetMoney(player.gold);
                    SetDiamond(player.diamond);
                }
            }
            

            DontDestroyOnLoad(this);
        }

        public void SetScore(float score)
        {
            numberScore += (int) score;
            txtScore.text = numberScore.ToString();
        }

        public void SetDiamond(float diamond)
        {
            numberDiamond += (int) diamond;
            txtDiamond.text = numberDiamond.ToString();
        }

        public void SetMoney(float gold)
        {
            numberGold += (int) gold;
            txtMoney.text = numberGold + " $";
        }

        private static void SaveData<T>(T obj, string key)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/saveData/";
            Directory.CreateDirectory(path);
            FileStream fileStream = new FileStream(path + key, FileMode.OpenOrCreate);
            formatter.Serialize(fileStream, obj);
            fileStream.Close();
        }

        private static T LoadData<T>(string key)
        {
            T data = default;
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/saveData/";
            if (File.Exists(path + key))
            {
                FileStream fileStream = new FileStream(path + key, FileMode.OpenOrCreate);
                data = (T) formatter.Deserialize(fileStream);
                fileStream.Close();
            }
            else
            {
                return data;
            }

            return data;
        }

        private void OnApplicationQuit()
        {
            var scoreDatas = new ScoreDataObj
            {
                currentScore = 0,
                diamond = playerData.scoreDataObj.diamond,
                money = playerData.scoreDataObj.money,
                highScore = playerData.scoreDataObj.highScore
            };
            SaveData(scoreDatas, ScoreData);
            var playerDatas = new PlayerDataObj
            {
                position = playerData.playerDataObj.position,
                characterSelection = playerData.playerDataObj.characterSelection,
                currentScenes = playerData.playerDataObj.currentScenes,
                saveAudio = playerData.playerDataObj.saveAudio,
                soundEffect = playerData.playerDataObj.soundEffect,
                soundMusic = playerData.playerDataObj.soundMusic
            };
            SaveData(playerDatas, PlayerData);
        }

        private void OnDisable()
        {
            DataService.CleanConnection();
        }
    }
}