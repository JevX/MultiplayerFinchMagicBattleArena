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
    private void SpawnPlayer()
    {
        Vector3 instantiatePosition = Vector3.zero; //TODO

        //GameObject playerGameobject = Instantiate(playerPrefab, instantiatePosition, Quaternion.identity, cameraAR);
        //PhotonView _photonView = playerGameobject.GetComponent<PhotonView>();
        GameObject playerGameobject2 = PhotonNetwork.Instantiate("PLAYER", instantiatePosition, Quaternion.identity);
        playerGameobject2.transform.SetParent(cameraAR);
        //if (PhotonNetwork.AllocateViewID(_photonView))
        //{
        //    object[] data = new object[]
        //    {
        //        playerGameobject.transform.position - mainObject.transform.position,
        //        playerGameobject.transform.rotation,
        //        _photonView.ViewID
        //    };
        //    
        //    RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        //    {
        //        Receivers = ReceiverGroup.Others,
        //        CachingOption = EventCaching.AddToRoomCache
//
        //    };
//
        //    SendOptions sendOptions = new SendOptions
        //    {
        //        Reliability = true
        //    };
//
        //    //Raise Events!
        //    PhotonNetwork.RaiseEvent((byte) RaiseEventCodes.PlayerSpawnEventCode, data, raiseEventOptions, sendOptions);
        //}
        //else
        //{
//
        //    Debug.Log("Failed to allocate a viewID");
        //    Destroy(playerGameobject);
        //}
    }
}
