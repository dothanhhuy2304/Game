using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game.GamePlay
{
    public class GameManager : FastSingleton<GameManager>
    {
        private const string ScoreData = "scoreData";
        private const string PlayerData = "playerData";
        public CharacterScriptTableObject listCharacter;
        public PlayerData playerData;
        [SerializeField] private TMPro.TextMeshProUGUI txtScore;
        [SerializeField] private TMPro.TextMeshProUGUI txtDiamond;
        [SerializeField] private TMPro.TextMeshProUGUI txtMoney;

        private void Start()
        {
            DontDestroyOnLoad(this);
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

            if (LoadData<ScoreDataObj>(ScoreData) == null)
            {
                playerData.scoreDataObj.currentScore = 0;
                playerData.scoreDataObj.diamond = 0;
                playerData.scoreDataObj.money = 0;
                playerData.scoreDataObj.highScore = 0;
                txtDiamond.text = 0.ToString();
                txtMoney.text = 0.ToString();
                txtScore.text = 0.ToString();
            }
            else
            {
                playerData.scoreDataObj.currentScore = 0;
                playerData.scoreDataObj.highScore = LoadData<ScoreDataObj>(ScoreData).highScore;
                SetScore(0f);
                SetMoney(LoadData<ScoreDataObj>(ScoreData).money);
                SetDiamond(LoadData<ScoreDataObj>(ScoreData).diamond);
            }
        }

        public void SetScore(float score)
        {
            playerData.scoreDataObj.currentScore += score;
            txtScore.text = playerData.scoreDataObj.currentScore.ToString(System.Globalization.CultureInfo.CurrentCulture);
        }

        public void SetDiamond(float diamond)
        {
            playerData.scoreDataObj.diamond += diamond;
            txtDiamond.text = playerData.scoreDataObj.diamond.ToString(System.Globalization.CultureInfo.CurrentCulture);
        }

        public void SetMoney(float money)
        {
            playerData.scoreDataObj.money += money;
            txtMoney.text = playerData.scoreDataObj.money + " $";
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
    }
}