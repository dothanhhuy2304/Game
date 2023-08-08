using UnityEngine;

[System.Serializable]
public class MyRoomInfo : MonoBehaviour
{
    public TMPro.TMP_Text roomName;
    public TMPro.TMP_Text currentPlayer;
    public Photon.Realtime.RoomInfo info;
    public UnityEngine.UI.Button buttonJoinRoom;
}
