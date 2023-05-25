using System.Linq;
using Script.Core;
using TMPro;
using UnityEngine;

namespace Script.GamePlay
{
    // public class UserPref
    // {
    //     public static string userId;
    //     public static int characterSelected;
    //     public static int currentScreen;
    //     public static int saveScreenPass;
    //     public static float[] currentPosition = new float[3];
    // }
    
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
                HuyManager.Instance.userId = HuyManager.Instance.GetCurrentPlayerProfile().Id;
                Debug.LogError("Exit data");
            }

            if (!string.IsNullOrEmpty(HuyManager.Instance.userId))
            {
                var playerData = HuyManager.Instance.GetCurrentPlayerData();
                if (playerData.PlayerId.Equals(HuyManager.Instance.userId))
                {
                    SetScore(playerData.score);
                    SetMoney(playerData.gold);
                    SetDiamond(playerData.diamond);
                    HuyManager.Instance.characterSelected = playerData.characterSelect;
                    HuyManager.Instance.currentScreen = playerData.levelId;
                    HuyManager.Instance.currentPosition[0] = playerData.positionX;
                    HuyManager.Instance.currentPosition[1] = playerData.positionY;
                    HuyManager.Instance.currentPosition[2] = playerData.positionZ;
                }
            }
            else
            {
                SetScore(0);
                SetMoney(0);
                SetDiamond(0);
                HuyManager.Instance.characterSelected = 0;
                HuyManager.Instance.currentScreen = 0;
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
            gameData.PlayerId = HuyManager.Instance.userId;
            gameData.characterSelect = HuyManager.Instance.characterSelected;
            gameData.levelId = HuyManager.Instance.currentScreen;
            gameData.positionX = HuyManager.Instance.currentPosition[0];
            gameData.positionY = HuyManager.Instance.currentPosition[1];
            gameData.positionZ = HuyManager.Instance.currentPosition[2];
            gameData.score = numberScore;
            gameData.gold = numberGold;
            gameData.diamond = numberDiamond;
            gameData.health = gameData.health;
            HuyManager.Instance.UpdateUserData(gameData);
        }

        private void OnDisable()
        {
            DataService.CleanConnection();
        }
    }
}