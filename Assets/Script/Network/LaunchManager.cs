using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Script.Core;
using Script.GamePlay;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    [HideInInspector] [SerializeField] private List<MyRoomInfo> cachedRoomList = new List<MyRoomInfo>();
    [SerializeField] private GameObject panelIntro;
    [SerializeField] private GameObject panelRoom;
    [SerializeField] private GameObject inRoom;
    [SerializeField] private TMP_Text playerName;
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
        playerName.text = HuyManager.GetCurrentPlayerProfile().UserName;
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
        if (PhotonNetwork.InLobby)
        {
            if (cachedRoomList.Count > 0)
            {
                panelIntro.SetActive(false);
                LogFeedback("<color=green>Something Wrong...</color>");
                string roomNames = System.Guid.NewGuid().ToString().Substring(0, 10);
                PhotonNetwork.JoinRandomOrCreateRoom(null, maxPlayerInRoom, MatchmakingMode.FillRoom, null, null,
                    roomNames);
            }
            else
            {
                LogFeedback("<color=red>Room name can't be blank!</color>");
            }
        }
    }

    /// <summary>
    /// Join a room by name
    /// If not exit create new room
    /// </summary>
    public void JoinRoomByName()
    {
        if (PhotonNetwork.InLobby)
        {
            if (!string.IsNullOrEmpty(roomName.text))
            {
                panelIntro.SetActive(false);
                LogFeedback("Something Wrong...");
                PhotonNetwork.JoinRoom(roomName.text);
                inRoom.SetActive(true);
            }
            else
            {
                LogFeedback("<color=red>Room name can't be blank!</Color>");
            }
        }
    }

    /// <summary>
    /// Create a room by roomName
    /// If room name had exit create new room
    /// </summary>
    /// 
    public void CreateRoom()
    {
        if (PhotonNetwork.InLobby)
        {
            if (!string.IsNullOrEmpty(roomName.text))
            {
                panelIntro.SetActive(false);
                LogFeedback("<color=green>CreateRoom...</color>");
                PhotonNetwork.JoinOrCreateRoom(roomName.text, new RoomOptions {IsOpen = true, MaxPlayers = maxPlayerInRoom}, TypedLobby.Default);
                inRoom.SetActive(true);
            }
            else
            {
                LogFeedback("<color=red>Room name can't be blank!</color>");
            }
        }
    }

    public void ShowListRoom()
    {
        if (PhotonNetwork.InLobby)
        {
            _isShowListRoom = !_isShowListRoom;
            parentSpawnListRoom.SetActive(_isShowListRoom);
            panelRoom.SetActive(false);
            listRoom.SetActive(true);
        }
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
            if (info.RemovedFromList || info.PlayerCount == info.MaxPlayers || !info.IsOpen)
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
        if (PhotonNetwork.InLobby)
        {
            panelIntro.SetActive(false);
            PhotonNetwork.JoinRoom(roomNames);
            inRoom.SetActive(true);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (PhotonNetwork.InLobby)
        {
            UpdateCurrentRoomList(roomList);
        }
    }

    public override void OnJoinedLobby()
    {
        LogFeedback("<color=green>Joined to lobby...</color>");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.InRoom)
        {
            startGame.SetActive(PhotonNetwork.IsMasterClient);
            txtRoomName.text = PhotonNetwork.CurrentRoom.Name + " " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + maxPlayerInRoom;
        }
    }

    public override void OnLeftLobby()
    {
        if (GameManager.instance)
        {
            Destroy(GameManager.instance.gameObject);
        }

        LoadingScreenManager.Instance.FadeLoadingScene(0, false);
        base.OnLeftLobby();
    }

    public override void OnLeftRoom()
    {
        if (GameManager.instance)
        {
            Destroy(GameManager.instance.gameObject);
        }

        LoadingScreenManager.Instance.FadeLoadingScene(0, false);
        base.OnLeftRoom();
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        switch (cause)
        {
            case DisconnectCause.None:
                LogFeedback("<color=red>None</color>");
                break;
            case DisconnectCause.ExceptionOnConnect:
                LogFeedback("<color=red>ExceptionOnConnect</color>");
                break;
            case DisconnectCause.DnsExceptionOnConnect:
                LogFeedback("<color=red>DnsExceptionOnConnect</color>");
                break;
            case DisconnectCause.ServerAddressInvalid:
                LogFeedback("<color=red>ServerAddressInvalid</color>");
                break;
            case DisconnectCause.Exception:
                LogFeedback("<color=red>Exception</color>");
                break;
            case DisconnectCause.ServerTimeout:
                LogFeedback("<color=red>ServerTimeout</color>");
                break;
            case DisconnectCause.ClientTimeout:
                LogFeedback("<color=red>ClientTimeout</color>");
                break;
            case DisconnectCause.DisconnectByServerLogic:
                LogFeedback("<color=red>DisconnectByServerLogic</color>");
                break;
            case DisconnectCause.DisconnectByServerReasonUnknown:
                LogFeedback("<color=red>DisconnectByServerReasonUnknown</color>");
                break;
            case DisconnectCause.InvalidAuthentication:
                LogFeedback("<color=red>InvalidAuthentication</color>");
                break;
            case DisconnectCause.CustomAuthenticationFailed:
                LogFeedback("<color=red>CustomAuthenticationFailed</color>");
                break;
            case DisconnectCause.AuthenticationTicketExpired:
                LogFeedback("<color=red>AuthenticationTicketExpired</color>");
                break;
            case DisconnectCause.MaxCcuReached:
                LogFeedback("<color=red>MaxCcuReached</color>");
                break;
            case DisconnectCause.InvalidRegion:
                LogFeedback("<color=red>InvalidRegion</color>");
                break;
            case DisconnectCause.OperationNotAllowedInCurrentState:
                LogFeedback("<color=red>OperationNotAllowedInCurrentState</color>");
                break;
            case DisconnectCause.DisconnectByClientLogic:
                LogFeedback("<color=red>DisconnectByClientLogic</color>");
                break;
            case DisconnectCause.DisconnectByOperationLimit:
                LogFeedback("<color=red>DisconnectByOperationLimit</color>");
                break;
            case DisconnectCause.DisconnectByDisconnectMessage:
                LogFeedback("<color=red>DisconnectByDisconnectMessage</color>");
                break;
            case DisconnectCause.ApplicationQuit:
                LogFeedback("<color=red>ApplicationQuit</color>");
                break;
            default:
                LogFeedback("<color=red>Undefined</color>");
                break;
        }

        UIManager.instance.DisconnectToServer();
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        LogFeedback("<color=red>OnJoinRandomFailed</color>: Next -> Create a new Room</color>");
        PhotonNetwork.JoinOrCreateRoom(System.Guid.NewGuid().ToString()
            .Substring(0, 10), new RoomOptions {IsOpen = true, MaxPlayers = maxPlayerInRoom}, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        LogFeedback($"<color=green>{PhotonNetwork.CurrentRoom.Name}</color>");
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


    // private IEnumerator DurationEndScene(float timeDuration, int sceneLoad)
    // {
    //     UnityEngine.SceneManagement.SceneManager.LoadScene(levelGame[0]);
    //     while (!UnityEngine.SceneManagement.SceneManager.GetSceneAt(sceneLoad).isLoaded)
    //     {
    //         yield return null;
    //     }
    //
    //     yield return new WaitForSeconds(timeDuration);
    //     float currentTime = 0;
    //     while (currentTime < 1f)
    //     {
    //         currentTime *= Time.deltaTime;
    //         var txt = feedBackText;
    //         Color t = new Color {r = 255, g = 255, b = 255, a = 0.5f};
    //         Color b = new Color {r = 0, g = 0, b = 0, a = 1f};
    //         txt.color = Color.Lerp(t, b, 1 * Time.deltaTime);
    //         yield return new WaitForSeconds(0.1f);
    //         StopCoroutine(_currentCoroutine);
    //     }
    //
    //     yield return null;
    //     LogFeedback("<color=red>Leave Room...</color>");
    //     panelIntro.SetActive(true);
    //     PhotonNetwork.LeaveRoom();
    //     PhotonNetwork.LeaveLobby();
    // }

    public void ButtonStartGame()
    {
        PhotonNetwork.LoadLevel(levelGame[currentScreen]);
    }

    /// <summary>
    /// Using to log message when next state
    /// </summary>
    /// <param name="message">Content message</param>
    public void LogFeedback(string message)
    {
        feedBackText.text = null;
        feedBackText.text = message;
    }

}
