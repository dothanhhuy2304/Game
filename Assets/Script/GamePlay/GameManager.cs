using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game.GamePlay
{
    public class GameManager : MonoBehaviour
    {
        private const string ScoreData = "scoreData";
        private const string PlayerData = "playerData";
        private static GameManager _instance;
        [SerializeField] private ScoreData scoreData;
        [SerializeField] private PlayerData playerData;
        [SerializeField] private TMPro.TextMeshProUGUI txtScore;
        [SerializeField] private TMPro.TextMeshProUGUI txtDiamond;
        [SerializeField] private TMPro.TextMeshProUGUI txtMoney;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(this);
            if (LoadData<PlayerDataObj>(PlayerData) == null)
            {
                playerData.playerDataObj.position = null;
                playerData.playerDataObj.characterSelection = 0;
                playerData.playerDataObj.currentScenes = 0;
                playerData.playerDataObj.saveAudio = false;
                playerData.playerDataObj.soundEffect = 0;
                playerData.playerDataObj.soundMusic = 0;
            }
            else
            {
                playerData.playerDataObj.position = LoadData<PlayerDataObj>(PlayerData).position;
                playerData.playerDataObj.characterSelection = LoadData<PlayerDataObj>(PlayerData).characterSelection;
                playerData.playerDataObj.currentScenes = LoadData<PlayerDataObj>(PlayerData).currentScenes;
                playerData.playerDataObj.saveAudio = LoadData<PlayerDataObj>(PlayerData).saveAudio;
                playerData.playerDataObj.soundEffect = LoadData<PlayerDataObj>(PlayerData).soundEffect;
                playerData.playerDataObj.soundMusic = LoadData<PlayerDataObj>(PlayerData).soundMusic;
            }

            if (LoadData<ScoreDataObj>(ScoreData) == null)
            {
                scoreData.scoreDataObj.currentScore = 0;
                scoreData.scoreDataObj.diamond = 0;
                scoreData.scoreDataObj.money = 0;
                scoreData.scoreDataObj.highScore = 0;
                txtDiamond.text = 0.ToString();
                txtMoney.text = 0.ToString();
                txtScore.text = 0.ToString();
            }
            else
            {
                scoreData.scoreDataObj.currentScore = 0;
                scoreData.scoreDataObj.highScore = LoadData<ScoreDataObj>(ScoreData).highScore;
                SetScore(0f);
                SetMoney(LoadData<ScoreDataObj>(ScoreData).money);
                SetDiamond(LoadData<ScoreDataObj>(ScoreData).diamond);
            }
        }

        public void SetScore(float score)
        {
            scoreData.scoreDataObj.currentScore += score;
            txtScore.text =
                scoreData.scoreDataObj.currentScore.ToString(System.Globalization.CultureInfo.CurrentCulture);
        }

        public void SetDiamond(float diamond)
        {
            scoreData.scoreDataObj.diamond += diamond;
            txtDiamond.text = scoreData.scoreDataObj.diamond.ToString(System.Globalization.CultureInfo.CurrentCulture);
        }

        public void SetMoney(float money)
        {
            scoreData.scoreDataObj.money += money;
            txtMoney.text = scoreData.scoreDataObj.money + " $";
        }

        private static void SaveData<T>(T obj, string key)
        {
            var formatter = new BinaryFormatter();
            var path = Application.persistentDataPath + "/saveData/";
            Directory.CreateDirectory(path);
            var fileStream = new FileStream(path + key, FileMode.Create);
            formatter.Serialize(fileStream, obj);
            fileStream.Close();
        }

        private static T LoadData<T>(string key)
        {
            T data = default;
            var formatter = new BinaryFormatter();
            var path = Application.persistentDataPath + "/saveData/";
            if (File.Exists(path + key))
            {
                var fileStream = new FileStream(path + key, FileMode.OpenOrCreate);
                data = (T) formatter.Deserialize(fileStream);
                fileStream.Close();
            }
            else
            {
                throw new Exception("Path not found");
            }

            return data;
        }

        private void OnApplicationQuit()
        {
            var scoreDatas = new ScoreDataObj
            {
                currentScore = 0,
                diamond = scoreData.scoreDataObj.diamond,
                money = scoreData.scoreDataObj.money,
                highScore = scoreData.scoreDataObj.highScore
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
    }
}