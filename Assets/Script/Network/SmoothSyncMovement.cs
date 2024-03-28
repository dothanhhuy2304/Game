// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmoothSyncMovement.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
//  Smoothed out movement for network gameobjects
// </summary>                                                                                             
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Smoothed out movement for network gameobjects
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class SmoothSyncMovement : Photon.Pun.MonoBehaviourPun, IPunObservable
    {
        private float SmoothingDelay = 0.1f;
        public void Awake()
        {
            bool observed = false;
            foreach (Component observedComponent in photonView.ObservedComponents)
            {
                if (observedComponent == this)
                {
                    observed = true;
                    break;
                }
            }
            if (!observed)
            {
                Debug.LogWarning(this + " is not observed by this object's photonView! OnPhotonSerializeView() in this class won't be used.");
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                //We own this player: send the others our data
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(transform.localScale);
            }
            else
            {
                //Network player, receive data
                _correctPlayerPos = (Vector3) stream.ReceiveNext();
                _correctPlayerRot = (Quaternion) stream.ReceiveNext();
                transform.localScale = (Vector3) stream.ReceiveNext();
            }
        }

        private Vector3 _correctPlayerPos = Vector3.zero; //We lerp towards this
        private Quaternion _correctPlayerRot = Quaternion.identity; //We lerp towards this

        public void Update()
        {
            if (!photonView.IsMine)
            {
                //Update remote player (smooth this, this looks good, at the cost of some accuracy)
                transform.position = Vector3.Lerp(transform.position, _correctPlayerPos, Time.deltaTime * SmoothingDelay);
                // ReSharper disable once Unity.InefficientPropertyAccess
                transform.rotation = Quaternion.Lerp(transform.rotation, _correctPlayerRot, Time.deltaTime * SmoothingDelay);
            }
        }

    }
}