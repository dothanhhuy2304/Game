using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private static PhotonManager _instance;
    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private TMP_Text messageError;
    [SerializeField] private byte maxPlayer;
    private List<MyRoomInfo> cachedRoomList = new List<MyRoomInfo>();
    [SerializeField] private GameObject listRoomUi;
    [SerializeField] private MyRoomInfo prefabRoomInfo;
    [SerializeField] private GameObject posSpawnRoom;
    private bool _isShowListRoom;
    [Space]
    [SerializeField] private LoadingUi loadingUiAnim;

    [SerializeField] private int currentScene= 1;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            PhotonManager[] currentInstanceExit = FindObjectsOfType<PhotonManager>();
            foreach (var cInstance in currentInstanceExit)
            {
                Destroy(cInstance);
            }
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = Application.version;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        //can loading anim
        loadingUiAnim.StartAnimationLoading();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        loadingUiAnim.StopAnimationLoading();
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomName.text))
        {
            loadingUiAnim.StartAnimationLoading();
            LogFeedback("Create Room...");
            PhotonNetwork.CreateRoom(roomName.text.ToLower(), new RoomOptions {IsOpen = true, MaxPlayers = maxPlayer}, TypedLobby.Default);
        }
        else
        {
            LogFeedback("<Color=Red>Room name can't be blank!</Color>");
        }
    }

    public void JoinRoomByName()
    {
        if (!string.IsNullOrEmpty(roomName.text))
        {
            loadingUiAnim.StartAnimationLoading();
            LogFeedback("Joining Room...");
            foreach (var roomInfo in cachedRoomList)
            {
                if (roomInfo.roomName.text == roomName.text.ToLower())
                {
                    PhotonNetwork.JoinRoom(roomName.text);
                }
                else
                {
                    LogFeedback("<Color=Red>Room name does not exist!</Color>");
                }
            }
        }
        else
        {
            LogFeedback("<Color=Red>Room name can't be blank!</Color>");
        }
    }

    public void JoinRandomRoom()
    {
        if (cachedRoomList.Count > 0)
        {
            loadingUiAnim.StartAnimationLoading();
            LogFeedback("Joining Room...");
            PhotonNetwork.JoinRandomOrCreateRoom();
        }
        else
        {
            LogFeedback("Join the failure room!");
        }
    }

    public void ShowListRoom()
    {
        _isShowListRoom = !_isShowListRoom;
        listRoomUi.SetActive(_isShowListRoom);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCurrentRoomList(roomList);
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
        LogFeedback("Discount to server...");
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        if (PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();
        loadingUiAnim.StopAnimationLoading();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
        loadingUiAnim.StopAnimationLoading();
        PhotonNetwork.JoinOrCreateRoom(System.Guid.NewGuid().ToString().Substring(0, 10),
            new RoomOptions {IsOpen = true, MaxPlayers = maxPlayer}, TypedLobby.Default);
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    private void UpdateCurrentRoomList(List<RoomInfo> roomList)
    {
        Debug.LogError("Number room exist" + roomList.Count);
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
                    int exist = cachedRoomList.FindIndex(t => t.info.Equals(info));
                    if (exist >= 0)
                    {
                        cachedRoomList[exist].roomName.text = info.Name;
                        cachedRoomList[exist].currentPlayer.text = $"{info.PlayerCount}/{info.MaxPlayers}";
                    }
                    else
                    {
                        MyRoomInfo btnJoin = Instantiate(prefabRoomInfo, posSpawnRoom.transform);
                        btnJoin.name = btnJoin.info.Name;
                        btnJoin.roomName.text = info.Name;
                        btnJoin.currentPlayer.text = $"{info.PlayerCount}/{info.MaxPlayers}";
                        btnJoin.btnJoin.onClick.AddListener(() => JoinRoom(info.Name));
                        btnJoin.gameObject.SetActive(true);
                        btnJoin.info = info;
                        cachedRoomList.Add(btnJoin);
                    }
                }
            }
        }
    }
    
    private void JoinRoom(string roomNames)
    {
        //controlPanel.SetActive(false);
        loadingUiAnim.StartAnimationLoading();
        PhotonNetwork.JoinRoom(roomNames);
    }

    private void ButtonStart()
    {
        PhotonNetwork.LoadLevel(loadingUiAnim.levelGame[currentScene]);
        loadingUiAnim.StopAnimationLoading();
    }
    
    // using for movement
    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //     if (stream.IsWriting)
    //     {
    //         stream.SendNext(rigidbody.position);
    //         stream.SendNext(rigidbody.position);
    //         stream.SendNext(rigidbody.position);
    //     }
    //     else
    //     {
    //         rigidbody.position = (Vector3) stream.ReceiveNext();
    //         rigidbody.rotation = (Quaternion) stream.ReceiveNext();
    //         rigidbody.velocity = (Vector3) stream.ReceiveNext();
    //
    //         float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.timestamp));
    //         rigidbody.position += rigidbody.velocity * lag;
    //     }
    // }

    private void LogFeedback(string message)
    {
        messageError.text = null;
        messageError.text = System.Environment.NewLine + message;
    }
}

internal class OnPhotonSerializeView
{
}
