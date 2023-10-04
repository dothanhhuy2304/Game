using System.Linq;
using Script.Core;
using TMPro;
using UnityEngine;

namespace Script.GamePlay
{
    
    public class GameManager : FastSingleton<GameManager>
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private TextMeshProUGUI txtScore;
        [SerializeField] private TextMeshProUGUI txtDiamond;
        [SerializeField] private TextMeshProUGUI txtMoney;
        [HideInInspector] public int numberScore;
        [HideInInspector ]public int numberGold;
        [HideInInspector] public int numberDiamond;
        public GameObject lobbyPanel;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {

            if (!DataService.GetConnection().Table<DataService.PlayerProfileData>().Any())
            {
                uiManager.PopupCreateAccount();
            }
            else
            {
                HuyManager.Instance.userId = HuyManager.Instance.GetCurrentPlayerProfile().Id;
                Debug.Log("Exit data");
            }

            if (!string.IsNullOrEmpty(HuyManager.Instance.userId))
            {
                var playerData = HuyManager.Instance.GetCurrentPlayerData();
                if (playerData.PlayerId.Equals(HuyManager.Instance.userId))
                {
                    SetScore(playerData.Score);
                    SetMoney(playerData.Gold);
                    SetDiamond(playerData.Diamond);
                    HuyManager.Instance.characterSelected = playerData.CharacterSelect;
                    HuyManager.Instance.currentScreen = playerData.LevelId;
                    HuyManager.Instance.currentPosition[0] = playerData.PositionX;
                    HuyManager.Instance.currentPosition[1] = playerData.PositionY;
                    HuyManager.Instance.currentPosition[2] = playerData.PositionZ;
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
            gameData.CharacterSelect = HuyManager.Instance.characterSelected;
            gameData.LevelId = HuyManager.Instance.currentScreen;
            gameData.PositionX = HuyManager.Instance.currentPosition[0];
            gameData.PositionY = HuyManager.Instance.currentPosition[1];
            gameData.PositionZ = HuyManager.Instance.currentPosition[2];
            gameData.Score = numberScore;
            gameData.Gold = numberGold;
            gameData.Diamond = numberDiamond;
            gameData.Health = gameData.Health;
            HuyManager.Instance.UpdateUserData(gameData);
        }

        private void OnDisable()
        {
            DataService.CleanConnection();
        }
    }
}