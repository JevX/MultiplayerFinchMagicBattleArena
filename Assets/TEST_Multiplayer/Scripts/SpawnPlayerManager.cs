using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private GameObject mainObject;

    [SerializeField] private Transform cameraAR;
    [SerializeField] private Text textDebug;
    
    public enum RaiseEventCodes
    {
        PlayerSpawnEventCode = 0
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void Update()
    {
        if (playerGameobject2 != null) textDebug.text = $"{name} {transform.rotation} + {transform.localRotation}";
    }

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    
    
    private void OnEvent(EventData photonEvent)
    {
        AddDebugText($"ивент {photonEvent.Code}");
        if (photonEvent.Code == (byte)RaiseEventCodes.PlayerSpawnEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            Vector3 receivedPosition = (Vector3)data[0];
            Quaternion receivedRotation = (Quaternion)data[1];

            GameObject player = Instantiate(playerPrefab, receivedPosition + mainObject.transform.position, receivedRotation);
            PhotonView photonView = player.GetComponent<PhotonView>();
            photonView.ViewID = (int)data[2];
        }
    }
    
    public override void OnJoinedRoom()
    {
        AddDebugText($"присоеденен к комнате {PhotonNetwork.CurrentRoom.Name}  {PhotonNetwork.CurrentRoom.Players.Count}");
        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();
        }
    }

    private void AddDebugText(string text)
    {
        textDebug.text = $"{textDebug.text}\n{text}";
    }

    private GameObject playerGameobject2 =null;
    private void SpawnPlayer()
    {
        Vector3 instantiatePosition = Vector3.zero; //TODO
        playerGameobject2 = PhotonNetwork.Instantiate("PLAYER", instantiatePosition, Quaternion.identity);
        playerGameobject2.transform.SetParent(cameraAR);
    }
}
