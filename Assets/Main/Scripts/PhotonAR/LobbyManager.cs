//23.04.2021 Roman Baranov

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Main.Scripts.PhotonAR.Roma
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [Header("Multiplayer Lobby Popup")]
        [SerializeField] private GameObject _remoteMultiplayerLobbyPopup = null;// Попап с подключением к матчу

        [Space(2)]

        [Header("Connection Status Popup")]
        [SerializeField] private GameObject _connectionStatusPopup = null;// Попап с информацией о текущем состоянии подключения
        [SerializeField] private Text _connectionStatusText = null;// Текст с текущим статусом подключения
        [SerializeField] private bool _showConnectionStatus = false;// Отображать ли статус подключения

        [Space(2)]

        [Header("Scenes To Load Names")]
        [SerializeField] private string _battleSceneName = null;
        [SerializeField] private string _sceneNameOnBackButton = null;

        #region UNITY Methods
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log($"PhotonNetwork.IsConnected = {PhotonNetwork.IsConnected}");

            if (PhotonNetwork.IsConnected)
            {
                // Инициализация элементов UI в случае, если установлено подключение к серверам Photon
                _remoteMultiplayerLobbyPopup.SetActive(true);
                _connectionStatusPopup.SetActive(false);

                //_loginPopup.SetActive(false);
            }
            else
            {
                // Инициализация элементов UI в случае, если нет подключения к серверам Photon
                _remoteMultiplayerLobbyPopup.SetActive(false);
                _connectionStatusPopup.SetActive(false);

               // _loginPopup.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_showConnectionStatus)
            {
                _connectionStatusText.text = $"Connection Status: {PhotonNetwork.NetworkClientState}";// Выводим текущее состояние подключения
            }

        }

        #endregion

        #region UI Callback Methods
        ///// <summary>
        ///// Присваивает игроку имя и подключается к серверам Photon
        ///// </summary>
        //public void OnEnterGameButtonPress()
        //{

        //    string playerName = _playerNameInputField.text;

        //    if (!string.IsNullOrEmpty(playerName))
        //    {
        //        _loginPopup.SetActive(false);
        //        _remoteMultiplayerLobbyPopup.SetActive(false);

        //        _showConnectionStatus = true;
        //        _connectionStatusPopup.SetActive(true);


        //        if (!PhotonNetwork.IsConnected)
        //        {
        //            PhotonNetwork.LocalPlayer.NickName = playerName;
        //            PhotonNetwork.ConnectUsingSettings();
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("Player name is invalid or empty!");
        //    }
        //}

        //TO DO! Возможно будет нужно добавить промежуточную сцену между лобби и боевой ареной
        /// <summary>
        /// Загружает сцену с игровой ареной (BattleArenaScene)
        /// </summary>
        public void QuickMatchButtonPress()
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScene");
            if (!string.IsNullOrEmpty(_battleSceneName))
            {
                SceneLoader.Instance.LoadScene(_battleSceneName);
            }
            else
            {
                Debug.Log("_battleSceneName is null or empty!");
            }
        }

        #endregion

        #region PHOTON Callback Methods

        /// <summary>
        /// Перегруженный колбэк при подключении к Internet
        /// </summary>
        public override void OnConnected()
        {
            Debug.Log("Connected to Internet");
        }

        /// <summary>
        /// Перегруженный колбэк при подключении к мастер серверу
        /// </summary>
        public override void OnConnectedToMaster()
        {
            //_loginPopup.SetActive(false);
            _connectionStatusPopup.SetActive(false);

            _remoteMultiplayerLobbyPopup.SetActive(true);


            Debug.Log($"{PhotonNetwork.LocalPlayer.NickName} is connected to Photon Server");
        }

        #endregion
    }
}
