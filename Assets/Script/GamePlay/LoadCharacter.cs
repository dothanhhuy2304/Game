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
        private GameObject character;
        private GameObject myPet;

        private void Awake()
        {
            GameManager.instance.lobbyPanel.SetActive(false);

            if (photonView.IsMine)
            {
                //characters[HuyManager.Instance.characterSelected].SetActive(true);
                photonView.RPC(nameof(SpawnPlayer), RpcTarget.All);
                photonView.RPC(nameof(GetComponent), RpcTarget.All);
            }

            if (GameManager.instance == null)
            {
                Instantiate(Resources.Load<GameObject>("GameManager"));
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
            character = PhotonNetwork.Instantiate(characters[HuyManager.Instance.characterSelected].name,
                characters[HuyManager.Instance.characterSelected].transform.position, Quaternion.identity);
            //HuyManager.Instance.IsLocalPlayer = character.GetComponent<CharacterController2D>();
            myPet = PhotonNetwork.Instantiate(pet.name, pet.transform.position, Quaternion.identity);
            //HuyManager.Instance.IsLocalPet = myPet.GetComponent<PetAI>();
        }

        [PunRPC]
        private void GetComponent()
        {
            HuyManager.Instance.IsLocalPlayer = character.GetComponent<CharacterController2D>();
            HuyManager.Instance.IsLocalPet = myPet.GetComponent<PetAI>();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // if (stream.IsWriting)
            // {
            //     stream.SendNext((GameObject) character);
            //     stream.SendNext((GameObject) myPet);
            // }
            // else
            // {
            //     character = (GameObject)stream.ReceiveNext();
            //     myPet = (GameObject) stream.ReceiveNext();
            //     HuyManager.Instance.IsLocalPlayer = character.GetComponent<CharacterController2D>();
            //     HuyManager.Instance.IsLocalPet = myPet.GetComponent<PetAI>();
            // }
        }
    }
}