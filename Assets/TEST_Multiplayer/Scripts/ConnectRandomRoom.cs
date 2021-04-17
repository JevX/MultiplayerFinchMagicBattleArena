using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class ConnectRandomRoom  : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button btnConnectRoom = null;
    
    private void Start()
    {
        btnConnectRoom.onClick.AddListener(OnConnectRoom);
    }

    private void OnConnectRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    
    
    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room" + Random.Range(0,1000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        //Creating the room
        PhotonNetwork.CreateRoom(randomRoomName,roomOptions);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        CreateAndJoinRoom();
    }
    
    public override void OnJoinedRoom()
    {
        Debug.Log( "Подключен "+ PhotonNetwork.CurrentRoom.Name);
        btnConnectRoom.gameObject.SetActive(false);
    }
    
}
