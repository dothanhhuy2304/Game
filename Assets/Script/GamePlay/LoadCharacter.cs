using Photon.Pun;
using Script.Core;
using Script.Player;
using UnityEngine;

namespace Script.GamePlay
{
    public class LoadCharacter : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField] private GameObject[] characters;
        [SerializeField] private GameObject pet;

        private void Awake()
        {
            if (GameManager.instance == null)
            {
                Instantiate(Resources.Load<GameObject>("GameManager"));
            }

            GameManager.instance.lobbyPanel.SetActive(false);

            if (photonView.IsMine)
            {
                //characters[HuyManager.Instance.characterSelected].SetActive(true);
                photonView.RPC(nameof(SpawnPlayer), RpcTarget.All);
            }

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
            GameObject player = PhotonNetwork.Instantiate(characters[HuyManager.Instance.characterSelected].name,
                characters[HuyManager.Instance.characterSelected].transform.position, Quaternion.identity);
            HuyManager.IsLocalPlayer = player.GetComponent<CharacterController2D>();
            GameObject myPet = PhotonNetwork.Instantiate(pet.name, pet.transform.position, Quaternion.identity);
            HuyManager.IsLocalPet = myPet.GetComponent<PetAI>();
            //player.SetActive(true);
            //photonView.gameObject.SetActive(true);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }
    }
}