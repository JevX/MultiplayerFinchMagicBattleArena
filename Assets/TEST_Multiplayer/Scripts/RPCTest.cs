using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Пример вызова заклинаний (методов) для мультиплеера, на этом принципе нужно сделать систему заклинаний
public class RPCTest : MonoBehaviour
{
    private PhotonView view;

    public ParticleSystem system;
    // Start is called before the first frame update
    void Start()
    {
        view = PhotonView.Get(this);
    }

    private void Update()
    {
        if (!view.IsMine) return;
        if (Input.GetKeyDown(KeyCode.Q))
        {
             view.RPC("PlayParticle", RpcTarget.All);
        }
    }

    [PunRPC]
    private void PlayParticle(PhotonMessageInfo info)
    {
        system.Play();
        print(info);
    } 
}
