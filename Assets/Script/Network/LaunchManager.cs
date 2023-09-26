using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Script.Core;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    private List<MyRoomInfo> cachedRoomList = new List<MyRoomInfo>();
    [SerializeField] private GameObject panelIntro;
    [SerializeField] private GameObject panelRoom;
    [SerializeField] private GameObject inRoom;
    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private byte maxPlayerInRoom;
    private bool _isShowListRoom;
    [SerializeField] private GameObject parentSpawnListRoom;
    [SerializeField] private MyRoomInfo myRoomInfoPrefab;
    [SerializeField] private GameObject startGame;
    [SerializeField] private int currentScreen = 1;
    [SerializeField] private GameObject listRoom;
    [SerializeField] private TMP_Text txtRoomName;
    [SerializeField] private TMP_Text feedBackText;
    [SerializeField] private string[] levelGame;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = Application.version;
        PhotonNetwork.ConnectUsingSettings();
    }
    
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// Join random a room
    /// </summary>
    public void JoinRandomRoom()
    {
        if (cachedRoomList.Count > 0)
        {
            panelIntro.SetActive(false);
            LogFeedback("Something Wrong...", true);
            string roomNames = System.Guid.NewGuid().ToString().Substring(0, 10);
            PhotonNetwork.JoinRandomOrCreateRoom(null, maxPlayerInRoom, MatchmakingMode.FillRoom, null, null,
                roomNames);
        }
        else
        {
            LogFeedback("<Color=Red>Room name can't be blank!</Color>", true);
        }
    }

    /// <summary>
    /// Join a room by name
    /// If not exit create new room
    /// </summary>
    public void JoinRoomByName()
    {
        if (!string.IsNullOrEmpty(roomName.text))
        {
            panelIntro.SetActive(false);
            LogFeedback("Something Wrong...", true);
            PhotonNetwork.JoinRoom(roomName.text);
            inRoom.SetActive(true);
        }
        else
        {
            LogFeedback("<Color=Red>Room name can't be blank!</Color>", true);
        }
    }

    /// <summary>
    /// Create a room by roomName
    /// If room name had exit create new room
    /// </summary>
    /// 
    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomName.text))
        {
            panelIntro.SetActive(false);
            LogFeedback("CreateRoom...", true);
            PhotonNetwork.JoinOrCreateRoom(roomName.text, new RoomOptions {IsOpen = true, MaxPlayers = maxPlayerInRoom},
                TypedLobby.Default);
            inRoom.SetActive(true);
        }
        else
        {
            Debug.LogError("This");
            LogFeedback("<Color=Red>Room name can't be blank!</Color>", true);
        }
    }

    public void ShowListRoom()
    {
        _isShowListRoom = !_isShowListRoom;
        parentSpawnListRoom.SetActive(_isShowListRoom);
        panelRoom.SetActive(false);
        listRoom.SetActive(true);
    }

    /// <summary>
    /// Update current room exits in the lobby if roomSlot great than maxPlayers destroy current room
    /// </summary>
    /// <param name="roomList">List current room exits</param>
    private void UpdateCurrentRoomList(List<RoomInfo> roomList)
    {
        Debug.LogError("Number room exits " + roomList.Count);
        foreach (var info in roomList)
        {
            if (info.RemovedFromList || info.PlayerCount == info.MaxPlayers)
            {
                for (int i = 0; i < cachedRoomList.Count; i++)
                {
                    if (cachedRoomList[i].info.Equals(info))
                    {
                        Destroy(cachedRoomList[i].gameObject);
                        cachedRoomList.Remove(cachedRoomList[i]);
                    }
                }
            }
            else
            {
                if (cachedRoomList.Count > 0)
                {
                    int exit = cachedRoomList.FindIndex(t => t.info.Equals(info));
                    if (exit >= 0)
                    {
                        cachedRoomList[exit].roomName.text = info.Name;
                        cachedRoomList[exit].currentPlayer.text = $"{info.PlayerCount}/{info.MaxPlayers}";
                    }
                    else
                    {
                        MyRoomInfo btnJoin = Instantiate(myRoomInfoPrefab, parentSpawnListRoom.transform);
                        btnJoin.name = btnJoin.info.Name;
                        btnJoin.buttonJoinRoom.onClick.AddListener(() => { JoinRoom(info.Name); });
                        btnJoin.roomName.text = info.Name;
                        btnJoin.currentPlayer.text = $"{info.PlayerCount}/{info.MaxPlayers}";
                        btnJoin.gameObject.SetActive(true);
                        btnJoin.info = info;
                        cachedRoomList.Add(btnJoin);
                    }
                }
                else
                {
                    MyRoomInfo btnJoin = Instantiate(myRoomInfoPrefab, parentSpawnListRoom.transform);
                    btnJoin.name = HuyManager.Instance.userId;
                    btnJoin.buttonJoinRoom.onClick.AddListener(() => { JoinRoom(info.Name); });
                    btnJoin.roomName.text = info.Name;
                    btnJoin.currentPlayer.text = $"{info.PlayerCount}/{info.MaxPlayers}";
                    btnJoin.gameObject.SetActive(true);
                    btnJoin.info = info;
                    cachedRoomList.Add(btnJoin);
                }
            }
        }
    }

    private void JoinRoom(string roomNames)
    {
        panelIntro.SetActive(false);
        PhotonNetwork.JoinRoom(roomNames);
        inRoom.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCurrentRoomList(roomList);
    }

    public override void OnJoinedLobby()
    {
        LogFeedback("Joined to lobby...", true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        startGame.SetActive(PhotonNetwork.IsMasterClient);
        txtRoomName.text = PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayerInRoom;
    }

    public override void OnLeftLobby()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
    }

    public override void OnLeftRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        if (PhotonNetwork.InRoom || PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.LeaveRoom();
        }
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room", true);
        PhotonNetwork.JoinOrCreateRoom(System.Guid.NewGuid().ToString()
            .Substring(0, 10), new RoomOptions {IsOpen = true, MaxPlayers = maxPlayerInRoom}, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        LogFeedback($"<Color=Green>{PhotonNetwork.CurrentRoom.Name}</Color>", true);
        LogFeedback($"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
        startGame.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        txtRoomName.text = PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayerInRoom;
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        //CheckStartGame();
    }

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue("IsPlayerReady", out object isPlayerReady))
            {
                if (!(bool) isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }


    //Beta need fix soon
    //Should leaveRoom and keep lobby
    //If disconnect lobby will connect again onConnectToMaster
    //Change scene will not work if loading not correct
    private Coroutine _currentCoroutine;

    public void LeaveRoom()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            _currentCoroutine = StartCoroutine(DurationEndScene(1, 0));
        }
    }

    private IEnumerator DurationEndScene(float timeDuration, int sceneLoad)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelGame[0]);
        while (!UnityEngine.SceneManagement.SceneManager.GetSceneAt(sceneLoad).isLoaded)
        {
            yield return null;
        }

        yield return new WaitForSeconds(timeDuration);
        float currentTime = 0;
        while (currentTime < 1f)
        {
            currentTime *= Time.deltaTime;
            var txt = feedBackText;
            Color t = new Color {r = 255, g = 255, b = 255, a = 0.5f};
            Color b = new Color {r = 0, g = 0, b = 0, a = 1f};
            txt.color = Color.Lerp(t, b, 1 * Time.deltaTime);
            yield return new WaitForSeconds(0.1f);
            StopCoroutine(_currentCoroutine);
        }

        yield return null;
        LogFeedback("Leave Room", true);
        panelIntro.SetActive(true);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
    }

    public void ButtonStartGame()
    {
        PhotonNetwork.LoadLevel(levelGame[currentScreen]);
    }
    
    /// <summary>
    /// Using to log message when next state
    /// </summary>
    /// <param name="message">Content message</param>
    public void LogFeedback(string message, bool reset = false)
    {
        feedBackText.text += "Connect status:" + PhotonNetwork.NetworkClientState;
    }
    
}
