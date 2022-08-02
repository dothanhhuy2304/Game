using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game.GamePlay
{
    public class GameManager : FastSingleton<GameManager>
    {
        private string path = Application.persistentDataPath + "/saveData/";
        private const string ScoreData = "scoreData";
        private const string PlayerData = "playerData";
        [SerializeField] private ScoreData scoreData;
        public PlayerData playerData;
        [SerializeField] private TMPro.TextMeshProUGUI txtScore;
        [SerializeField] private TMPro.TextMeshProUGUI txtDiamond;
        [SerializeField] private TMPro.TextMeshProUGUI txtMoney;

        private void Start()
        {
            DontDestroyOnLoad(this);
            if (LoadData<PlayerDataObj>(PlayerData, path) == null)
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
                if (LoadData<PlayerDataObj>(PlayerData, path).position == null)
                {
                    playerData.playerDataObj.position = new[] {-4.95f, -4f, 0};
                }
                else
                {
                    playerData.playerDataObj.position = LoadData<PlayerDataObj>(PlayerData, path).position;
                }

                playerData.playerDataObj.characterSelection = LoadData<PlayerDataObj>(PlayerData, path).characterSelection;
                playerData.playerDataObj.currentScenes = LoadData<PlayerDataObj>(PlayerData, path).currentScenes;
                playerData.playerDataObj.saveAudio = LoadData<PlayerDataObj>(PlayerData, path).saveAudio;
                playerData.playerDataObj.soundEffect = LoadData<PlayerDataObj>(PlayerData, path).soundEffect;
                playerData.playerDataObj.soundMusic = LoadData<PlayerDataObj>(PlayerData, path).soundMusic;
            }

            if (LoadData<ScoreDataObj>(ScoreData, path) == null)
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
                scoreData.scoreDataObj.highScore = LoadData<ScoreDataObj>(ScoreData, path).highScore;
                SetScore(0f);
                SetMoney(LoadData<ScoreDataObj>(ScoreData, path).money);
                SetDiamond(LoadData<ScoreDataObj>(ScoreData, path).diamond);
            }
        }

        public void SetScore(float score)
        {
            scoreData.scoreDataObj.currentScore += score;
            txtScore.text = scoreData.scoreDataObj.currentScore.ToString(System.Globalization.CultureInfo.CurrentCulture);
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

        private static void SaveData<T>(T obj, string key, string path)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Directory.CreateDirectory(path);
            FileStream fileStream = new FileStream(path + key, FileMode.OpenOrCreate);
            formatter.Serialize(fileStream, obj);
            fileStream.Close();
        }

        private static T LoadData<T>(string key, string path)
        {
            T data = default;
            BinaryFormatter formatter = new BinaryFormatter();
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
                diamond = scoreData.scoreDataObj.diamond,
                money = scoreData.scoreDataObj.money,
                highScore = scoreData.scoreDataObj.highScore
            };
            SaveData(scoreDatas, ScoreData, path);
            var playerDatas = new PlayerDataObj
            {
                position = playerData.playerDataObj.position,
                characterSelection = playerData.playerDataObj.characterSelection,
                currentScenes = playerData.playerDataObj.currentScenes,
                saveAudio = playerData.playerDataObj.saveAudio,
                soundEffect = playerData.playerDataObj.soundEffect,
                soundMusic = playerData.playerDataObj.soundMusic
            };
            SaveData(playerDatas, PlayerData, path);
        }
    }
}