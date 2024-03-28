using Photon.Pun;
using UnityEngine;

namespace Script.Player
{
    public class RbSmoothSyncMovement : MonoBehaviourPun, IPunObservable
    {
        private Rigidbody2D Rb => GetComponent<Rigidbody2D>();
        private float SmoothingDelay = 0.1f;

        private Vector3 latestPos;
        private Quaternion latestRot;
        private Vector3 velocity;
        private float angularVelocity;

        private bool valuesReceived;


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                //We own this player: send the others our data
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(transform.localScale);
                stream.SendNext(Rb.velocity);
                stream.SendNext(Rb.angularVelocity);
            }
            else
            {
                //Network player, receive data
                latestPos = (Vector3) stream.ReceiveNext();
                latestRot = (Quaternion) stream.ReceiveNext();
                transform.localScale = (Vector3) stream.ReceiveNext();
                velocity = (Vector3) stream.ReceiveNext();
                angularVelocity = (float) stream.ReceiveNext();

                valuesReceived = true;
            }
        }

        private void Update()
        {
            if (!photonView.IsMine && valuesReceived)
            {
                //Update Object position and Rigidbody parameters
                transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * SmoothingDelay);
                transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * SmoothingDelay);
                Rb.velocity = velocity;
                Rb.angularVelocity = angularVelocity;
            }
        }

        private void OnCollisionEnter(Collision contact)
        {
            if (!photonView.IsMine)
            {
                Transform collisionObjectRoot = contact.transform.root;
                if (collisionObjectRoot.CompareTag("Player"))
                {
                    //Transfer PhotonView of Rigidbody to our local player
                    photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                }
            }
        }
    }
}