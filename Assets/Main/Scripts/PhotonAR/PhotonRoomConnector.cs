using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Scripts.PhotonAR
{
    public class PhotonRoomConnector : MonoBehaviourPunCallbacks
    {
        [Header("Панели интерфейса")]
        [SerializeField] private GameObject currentPanelUI = null;
        [SerializeField] private GameObject nextPanelUI = null;
        
        [Header("Пользовательский ввод")]
        [SerializeField] private Button btnConnectRoom = null;
        [SerializeField] private InputField nameField = null;
        [SerializeField] private InputField nameRoomField = null;
        
        private void Start()
        {
            btnConnectRoom.onClick.AddListener(OnConnectRoom);
        }
        
        private void OnConnectRoom()
        {
            PhotonNetwork.LocalPlayer.NickName =
                string.IsNullOrEmpty(nameField.text) ? $"Player {Random.Range(100000, 999999)}" : nameField.text;

            if (string.IsNullOrEmpty(nameRoomField.text))
            {
                PhotonNetwork.JoinRandomRoom();
                return;
            }
            
            CreateAndJoinRoom(nameRoomField.text);
        }
    
    
        private void CreateAndJoinRoom(string roomName)
        {
            if (PhotonNetwork.IsConnected)
            {
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = 2;
                TypedLobby typedLobby = new TypedLobby(nameRoomField.text, LobbyType.Default);
            
                PhotonNetwork.JoinOrCreateRoom(roomName,roomOptions, typedLobby);
            } else {} // TODO добавить месседж об ошибке
        }

        private void SetActiveNextUI(bool isActive)
        {
            currentPanelUI.SetActive(!isActive);
            nextPanelUI.SetActive(isActive);
        }

        
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log(message);
            CreateAndJoinRoom($"Room {Random.Range(1000, 10000)}");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            SetActiveNextUI(false);
            // TODO добавить месседж об ошибке
        }

        public override void OnJoinedRoom()
        {
            Debug.Log( "Подключен к комнате " + PhotonNetwork.CurrentRoom.Name);
            if (PhotonNetwork.IsMasterClient)
            {
                SetActiveNextUI(true);
            }
        }

    }
}
