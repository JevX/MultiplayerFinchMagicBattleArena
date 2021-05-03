//24.04.2021 Roman Baranov

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using MAIN.Scripts.GameSettings;
using UnityEngine.SceneManagement;
using Main.Scripts;

namespace MAIN.Scripts.UI
{
    public class GameModeLoader : MonoBehaviour
    {
        #region VARIABLES
        [Header("Singleplayer Mode")]
        [SerializeField] private string _singleplayerSceneName = null;
        [SerializeField] private Button _singlePlayerModeButton = null;

        [Space(2)]

        [Header("Multiplayer Modes")]
        [SerializeField] private string _backToBackSceneName = null;

        [SerializeField] private Button _multiplayerModeButton = null;
        [SerializeField] private Button _multiplayerTypeBackToBackButton = null;

        [Space(2)]

        [SerializeField] private string _remoteSceneName = null;
        [SerializeField] private Button _multiplayerTypeRemoteConnectButton = null;

        [Header("Photon Connection Timeout")]
        [SerializeField] private float _waitToConnect = 10;
        [SerializeField] private Text _connectionStatusPopupText = null;

        private PlayerSettings _playerSettingsSO = null;

        #endregion

        #region UNITY Methods
        private void Awake()
        {
            UIInterfaceController.Instance.OpenInterface(InterfaceType.GameSelection_PlayerLoginPopup);

            _singlePlayerModeButton.onClick.AddListener(OnSingleplayerButtonPress);
            _multiplayerModeButton.onClick.AddListener(OnMultiplayerButtonPress);
            _multiplayerTypeBackToBackButton.onClick.AddListener(OnBackToBackMultiplayerButtonPress);
            _multiplayerTypeRemoteConnectButton.onClick.AddListener(OnRemoteMultiplayerButtonPress);
        }

        private void Start()
        {
            _playerSettingsSO = Resources.Load<PlayerSettings>("ScriptableObjects/PlayerSettings");
        }

        private void OnDestroy()
        {
            _singlePlayerModeButton.onClick.RemoveAllListeners();
            _multiplayerModeButton.onClick.RemoveAllListeners();
            _multiplayerTypeBackToBackButton.onClick.RemoveAllListeners();
            _multiplayerTypeRemoteConnectButton.onClick.RemoveAllListeners();
        }
        #endregion

        #region UI Callback Methods
        /// <summary>
        /// Загружает одиночный режим игры (сцена ARDemoScene)
        /// </summary>
        private void OnSingleplayerButtonPress()
        {
            // Загружаю сцену 
            if (!string.IsNullOrEmpty(_singleplayerSceneName))
            {
                SceneLoadManager.Instance.LoadScene(_singleplayerSceneName);
                //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_singleplayerSceneName);
            }
            else
            {
                Debug.Log("_singleplayerSceneName is null or empty!");
            }
        }

        /// <summary>
        /// Подключает к Photon
        /// </summary>
        private void OnMultiplayerButtonPress()
        {
            ConnectToPhoton();
        }

        /// <summary>
        /// Загружает сетевой режим игры Remote Multiplayer
        /// </summary>
        private void OnRemoteMultiplayerButtonPress()
        {
            // Загружаю сцену
            if (!string.IsNullOrEmpty(_remoteSceneName))
            {
                SceneLoadManager.Instance.LoadScene(_remoteSceneName);
                //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_remoteSceneName);
            }
            else
            {
                Debug.Log("_remoteSceneName is null or empty!");
            }
        }

        /// <summary>
        /// TO DO! Загружает сетевой режим игры Back to Back Multiplayer
        /// </summary>
        private void OnBackToBackMultiplayerButtonPress()
        {
            // Загружаю сцену TO DO
            if (!string.IsNullOrEmpty(_backToBackSceneName))
            {
                //SceneLoadManager.Instance.LoadScene(_backToBackSceneName);
                Debug.Log("TO DO! Back to Back Multiplayer mode need to be implemented!");
            }
            else
            {
                Debug.Log("_backToBackSceneName is null or empty!");
            }
        }
        #endregion

        #region PRIVATE PHOTON Methods
        /// <summary>
        /// Подключает к серверам Photon
        /// </summary>
        private void ConnectToPhoton()
        {
            if (!PhotonNetwork.IsConnected) StartCoroutine(ConnectionToPhoton());
        }

        /// <summary>
        /// Курутина для ConnectToPhoton(), подключающая игрока к Photon
        /// </summary>
        /// <returns></returns>
        private IEnumerator ConnectionToPhoton()
        {
            UIInterfaceController.Instance.OpenInterface(InterfaceType.GameSelection_ConnectionStatusPopup);

            float currentWait = 0;
            PhotonNetwork.GameVersion = Application.version; //TODO возможно убрать 

            // Присваиваю имя игроку
            PhotonNetwork.LocalPlayer.NickName = _playerSettingsSO.playerName;

            // Добавляю индекс аватара в Hashtable для LocalPlayer.CustomProperties
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add("Avatar", _playerSettingsSO.playerAvatarSpriteIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            PhotonNetwork.ConnectUsingSettings();

            while (currentWait < _waitToConnect)
            {
                if (PhotonNetwork.IsConnected) break;
                _connectionStatusPopupText.text = $"Connection Status: {PhotonNetwork.NetworkClientState}";
                currentWait += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            if (!PhotonNetwork.IsConnected)
            {
                _connectionStatusPopupText.text = $"Connection Error: {PhotonNetwork.NetworkClientState}";
                UIInterfaceController.Instance.OpenInterface(InterfaceType.GameSelection_GameModesPopup); // TODO возможно добавить сообщение об ошибке
            }
        }
        #endregion
    }
}
