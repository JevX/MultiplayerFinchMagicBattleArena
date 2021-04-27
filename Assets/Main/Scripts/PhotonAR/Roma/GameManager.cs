//Roman Baranov 
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Inform Panel Popup")]
    [SerializeField] private GameObject _connectionInformPanelPopup = null;
    [SerializeField] private Text _connectionInformPanelPopupText = null;

    [Space(2)]

    [Header("Buttons")]
    [SerializeField] private GameObject _searchForGamesButton = null;
    [SerializeField] private GameObject _quitMatchButton = null;

    [Space(2)]

    [Header("Scenes To Load Names")]
    [SerializeField] private string _sceneNameOnMatchQuit = null;

    // Start is called before the first frame update
    void Start()
    {
        _connectionInformPanelPopup.SetActive(true);
        _connectionInformPanelPopupText.text = "Search For Games To BATTLE!";
    }

    #region UI Callback Methods

    /// <summary>
    /// Подключает игрока к произвольной существующей комнате
    /// </summary>
    public void JoinRandomRoom()
    {
        _connectionInformPanelPopupText.text = "Searching for available rooms...";

        PhotonNetwork.JoinRandomRoom();

        _searchForGamesButton.SetActive(false);
    }

    /// <summary>
    /// Выходит из комнаты и загружает лобби при нажатии на кнопку QuitMatch
    /// </summary>
    public void OnQuitMatchButtonPress()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneLoader.Instance.LoadScene(_sceneNameOnMatchQuit);
        }
        
    }    
    #endregion


    #region PHOTON Callbsck Methods

    /// <summary>
    /// Перегруженный коллбэк OnJoinRandomFailed. Если не удалось подключиться к комнате, создает новую произвольную и подключается к ней.
    /// </summary>
    /// <param name="returnCode">Код ошибки при неудачном подключении</param>
    /// <param name="message">Сообщение ошибки при неудачном подключении</param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        _connectionInformPanelPopupText.text = message;

        CreateAndJoinRoom();
    }

    /// <summary>
    /// Перегруженный коллбэк OnJoinedRoom. Вызывается в момент подключения игрока к комнате.
    /// </summary>
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            _connectionInformPanelPopupText.text = $"Joined to {PhotonNetwork.CurrentRoom.Name}\nWaiting for other player...";
        }
        else
        {
            _connectionInformPanelPopupText.text = $"Joined to {PhotonNetwork.CurrentRoom.Name}";
            StartCoroutine(DeactivateAfterSeconds(_connectionInformPanelPopup, 2.0f));
        }

        Debug.Log($"Joined to {PhotonNetwork.CurrentRoom.Name}");
    }

    /// <summary>
    /// Перегруженный коллбэк OnPlayerEnteredRoom. Вызывается в момент подключения нового игрока к комнате.
    /// </summary>
    /// <param name="newPlayer">Имя подключившегося игрока</param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} joined to {PhotonNetwork.CurrentRoom.Name}.\nPlayer count {PhotonNetwork.CurrentRoom.PlayerCount}");
        _connectionInformPanelPopupText.text = $"{newPlayer.NickName} joined to {PhotonNetwork.CurrentRoom.Name}.\nPlayer count {PhotonNetwork.CurrentRoom.PlayerCount}";

        StartCoroutine(DeactivateAfterSeconds(_connectionInformPanelPopup, 2.0f));
    }

    /// <summary>
    /// Перегруженный коллбэк OnLeftRoom. Загружает сцену с лобби при выходе из комнаты
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneLoader.Instance.LoadScene("RemoteMultyplayerLobbyScene");
    }
    #endregion


    #region PRIVATE Methods
    /// <summary>
    /// Создает произвольную комнату и присоединяется к ней
    /// </summary>
    private void CreateAndJoinRoom()
    {
        string randomRoomName = $"Room {Random.Range(0, 10000)}";

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        // Создаю комнату
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    /// <summary>
    /// Курутина, которая отключает объект через заданное количество секунд
    /// </summary>
    /// <param name="gameObject">Отключаемый объект</param>
    /// <param name="seconds">Таймер до отключения объекта в секундах</param>
    /// <returns></returns>
    private IEnumerator DeactivateAfterSeconds(GameObject gameObject, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        gameObject.SetActive(false);
    }

    #endregion
}
