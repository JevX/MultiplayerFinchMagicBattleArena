// Created by Alexander Detkov
// Modifyed by Roman Baranov 07.05.2021

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Scripts.PhotonAR
{
    public class PhotonRoomConnector : MonoBehaviourPunCallbacks
    {
        #region VARIABLES
        [Header("Search Game Button")]
        [SerializeField] private Button _searchGamesButton = null;

        //[SerializeField] private GameObject currentPanelUI = null;
        //[SerializeField] private GameObject nextPanelUI = null;

        //[Header("Пользовательский ввод")]
        //[SerializeField] private Button btnConnectRoom = null;
        //[SerializeField] private InputField nameField = null;
        //[SerializeField] private InputField nameRoomField = null;
        #endregion

        #region UNITY Methods
        private void Start()
        {
            _searchGamesButton.onClick.AddListener(OnConnectRoom);
        }

        private void OnDestroy()
        {
            _searchGamesButton?.onClick.RemoveAllListeners();
        }
        #endregion

        #region PUBLIC Methods
        /// <summary>
        /// Перегруженный callback OnJoinRandomFailed. Создает комнату с произвольным именем и присоединяется к ней
        /// </summary>
        /// <param name="returnCode">Код ошибки при неудачном подключении к комнате</param>
        /// <param name="message">Сообщение при неудачном подключении к комнате</param>
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log(message);
            CreateAndJoinRoom(Random.Range(1000, 10000).ToString());
        }

        //public override void OnJoinRoomFailed(short returnCode, string message)
        //{
        //    SetActiveNextUI(false);
        //    // TODO добавить месседж об ошибке
        //}

        /// <summary>
        /// Перегруженный callback OnJoinedRoom. Выводит в консоль информацию о подключении к комнате
        /// </summary>
        public override void OnJoinedRoom()
        {
            Debug.Log("Подключен к комнате " + PhotonNetwork.CurrentRoom.Name);
            //SetActiveNextUI(true);
        }
        #endregion

        #region PRIVATE Methods
        /// <summary>
        /// Подключается к произвольной комнате
        /// </summary>
        private void OnConnectRoom()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        /// <summary>
        /// Создает комнату с произвольным именем и подключается к ней
        /// </summary>
        /// <param name="roomName">Название комнаты</param>
        private void CreateAndJoinRoom(string roomName)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = 2;
                TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default);

                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby);
            }
            else { } // TODO добавить месседж об ошибке
        }

        //private void OnConnectRoom()
        //{
        //    PhotonNetwork.LocalPlayer.NickName =
        //        string.IsNullOrEmpty(nameField.text) ? $"Player {Random.Range(100000, 999999)}" : nameField.text;

        //    if (string.IsNullOrEmpty(nameRoomField.text))
        //    {
        //        PhotonNetwork.JoinRandomRoom();
        //        return;
        //    }

        //    CreateAndJoinRoom(nameRoomField.text);
        //}

        //private void SetActiveNextUI(bool isActive)
        //{
        //    currentPanelUI.SetActive(!isActive);
        //    nextPanelUI.SetActive(isActive);
        //}
        #endregion
    }
}
