using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerNetwokControl : MonoBehaviourPunCallbacks
{
    public static GameObject IsLocalPlayer;
    public static PhotonView view;

    private void Awake()
    {
        if (photonView.IsMine)
        {
                IsLocalPlayer = gameObject;
                view = photonView;
            DontDestroyOnLoad(gameObject);
        }
    }
}
