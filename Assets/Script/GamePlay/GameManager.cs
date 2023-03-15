using System.Linq;
using TMPro;
using UnityEngine;

namespace Game.GamePlay
{
    public class UserPref
    {
        public static string userId;
        public static int characterSelected;
        public static int currentScreen;
        public static float[] currentPosition = new float[3];
    }
    
    public class GameManager : FastSingleton<GameManager>
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private TextMeshProUGUI txtScore;
        [SerializeField] private TextMeshProUGUI txtDiamond;
        [SerializeField] private TextMeshProUGUI txtMoney;
        //
        [HideInInspector] public int numberScore;
        [HideInInspector ]public int numberGold;
        [HideInInspector] public int numberDiamond;

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

            if (!string.IsNullOrEmpty(UserPref.userId))
            {
                var listPlayerData = DataService.GetConnection().Table<DataService.GameData>().ToList();
                foreach (var player in listPlayerData)
                {
                    if (player.PlayerId.Equals(UserPref.userId))
                    {
                        SetScore(player.score);
                        SetMoney(player.gold);
                        SetDiamond(player.diamond);
                        UserPref.characterSelected = player.characterSelect;
                        UserPref.currentScreen = player.levelId;
                        UserPref.currentPosition[0] = player.positionX;
                        UserPref.currentPosition[1] = player.positionY;
                        UserPref.currentPosition[2] = player.positionZ;
                    }
                }
            }
            else
            {
                SetScore(0);
                SetMoney(0);
                SetDiamond(0);
                UserPref.characterSelected = 0;
                UserPref.currentScreen = 0;
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

        // private static void SaveData<T>(T obj, string key)
        // {
        //     BinaryFormatter formatter = new BinaryFormatter();
        //     string path = Application.persistentDataPath + "/saveData/";
        //     Directory.CreateDirectory(path);
        //     FileStream fileStream = new FileStream(path + key, FileMode.OpenOrCreate);
        //     formatter.Serialize(fileStream, obj);
        //     fileStream.Close();
        // }

        // private static T LoadData<T>(string key)
        // {
        //     T data = default;
        //     BinaryFormatter formatter = new BinaryFormatter();
        //     string path = Application.persistentDataPath + "/saveData/";
        //     if (File.Exists(path + key))
        //     {
        //         FileStream fileStream = new FileStream(path + key, FileMode.OpenOrCreate);
        //         data = (T) formatter.Deserialize(fileStream);
        //         fileStream.Close();
        //     }
        //     else
        //     {
        //         return data;
        //     }
        //
        //     return data;
        // }

        private void OnApplicationQuit()
        {
            DataService.GameData gameData = new DataService.GameData();
            gameData.PlayerId = UserPref.userId;
            gameData.characterSelect = UserPref.characterSelected;
            gameData.levelId = UserPref.currentScreen;
            gameData.positionX = UserPref.currentPosition[0];
            gameData.positionY = UserPref.currentPosition[1];
            gameData.positionZ = UserPref.currentPosition[2];
            gameData.score = numberScore;
            gameData.gold = numberGold;
            gameData.diamond = numberDiamond;
            gameData.health = gameData.health;
            DataService.GetConnection().Table<DataService.GameData>().Connection.Update(gameData);
        }

        private void OnDisable()
        {
            DataService.CleanConnection();
        }
    }
}