using Photon.Pun;
using Script.Core;
using Script.Player;
using UnityEngine;

namespace Script.GamePlay
{
    public class LoadCharacter : MonoBehaviourPunCallbacks,IPunObservable
    {
        [SerializeField] private GameObject[] characters;

        private void Awake()
        {
            if (GameManager.instance == null)
            {
                Instantiate(Resources.Load<GameObject>("GameManager"));
            }

            //if (PlayerNetwokControl.IsLocalPlayer)
            //{
                //characters[HuyManager.Instance.characterSelected].SetActive(true);
                SpawnPlayer();
            //}

            AudioManager.instance.Plays_Music("Music_Game");

            if (!UIManager.instance.scoreUi.activeSelf)
            {
                UIManager.instance.scoreUi.SetActive(true);
                UIManager.instance.btnBackToMenu.gameObject.SetActive(true);
                UIManager.instance.btnRestart.gameObject.SetActive(true);
            }
        }

        [PunRPC]
        private void SpawnPlayer()
        {
            PhotonNetwork.Instantiate(characters[HuyManager.Instance.characterSelected].name,
                characters[HuyManager.Instance.characterSelected].transform.position, Quaternion.identity);
            //player.SetActive(true);
            //photonView.gameObject.SetActive(true);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }
    }
}