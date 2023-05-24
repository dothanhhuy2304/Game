using System.Linq;
using TMPro;
using UnityEngine;

namespace Script.GamePlay
{
    public class UserPref
    {
        public static string userId;
        public static int characterSelected;
        public static int currentScreen;
        public static int saveScreenPass;
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
                var playerData = DataService.GetConnection().Table<DataService.GameData>().FirstOrDefault();
                if (playerData.PlayerId.Equals(UserPref.userId))
                {
                    SetScore(playerData.score);
                    SetMoney(playerData.gold);
                    SetDiamond(playerData.diamond);
                    UserPref.characterSelected = playerData.characterSelect;
                    UserPref.currentScreen = playerData.levelId;
                    UserPref.currentPosition[0] = playerData.positionX;
                    UserPref.currentPosition[1] = playerData.positionY;
                    UserPref.currentPosition[2] = playerData.positionZ;
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