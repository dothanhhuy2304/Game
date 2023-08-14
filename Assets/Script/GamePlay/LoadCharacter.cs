using Photon.Pun;
using Script.Core;
using Script.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.GamePlay
{
    public class LoadCharacter : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject[] characters;
        [SerializeField] private GameObject pet;

        private void Awake()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene("Client");
            }
            
            GameManager.instance.lobbyPanel.SetActive(false);

            if (characters == null || pet == null)
            {
                SceneManager.LoadScene("Client");
            }
            else
            {
                // if (photonView.IsMine)
                // {
                //     //characters[HuyManager.Instance.characterSelected].SetActive(true);
                //     photonView.RPC(nameof(SpawnPlayer), RpcTarget.All);
                // }
                if (CharacterController2D.IsLocalPlayer == null)
                {
                    //photonView.RPC(nameof(SpawnPlayer), RpcTarget.All);
                    PhotonNetwork.Instantiate(characters[HuyManager.Instance.characterSelected].name,
                        characters[HuyManager.Instance.characterSelected].transform.position, Quaternion.identity);
                }

                if (PetAI.IsLocalPet == null)
                {
                    PhotonNetwork.Instantiate(pet.name, pet.transform.position, Quaternion.identity);
                }
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

        // [PunRPC]
        // private void SpawnPlayer()
        // {
        //     PhotonNetwork.Instantiate(characters[HuyManager.Instance.characterSelected].name,
        //         characters[HuyManager.Instance.characterSelected].transform.position, Quaternion.identity);
        // }
    }
}