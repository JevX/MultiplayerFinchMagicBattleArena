using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ConnectPhoton : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button btnConnectNetwork = null;


    private void Start()
    {
        btnConnectNetwork.onClick.AddListener(OnConnectPhotonNetwork);
    }

    private void OnConnectPhotonNetwork()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = Random.Range(0,10000).ToString();
            PhotonNetwork.ConnectUsingSettings();
        }
        
    }
    
    public override void OnConnected()
    {
        Debug.Log("Мы подключены к фотону");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " подключен к серверу");
        SceneLoader.Instance.LoadScene("TestARMultiplayer");
    }

}
