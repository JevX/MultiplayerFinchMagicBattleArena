//24.04.2021 Roman Baranov
using UnityEngine;

public class GameModeLoader : MonoBehaviour
{
    [Header("Game Modes Popup")]
    [SerializeField] private GameObject _gameModesPopup = null;// Блок с выбором игровых режимов - одиночный / мультиплеер
    [SerializeField] private string _singleplayerSceneName = null;

    [Space(2)]

    [Header("Multiplayer Types Popup")]
    [SerializeField] private GameObject _multiplayerTypesPopup = null;// Блок с выбором типа мультиплеера - спина к спине (облачные якоря) / удаленное соединение
    [SerializeField] private string _backToBackSceneName = null;
    [SerializeField] private string _remoteSceneName = null;

    private void Awake()
    {
        _gameModesPopup.SetActive(true);
        _multiplayerTypesPopup.SetActive(false);
    }

    #region UI Callback Methods
    /// <summary>
    /// Загружает одиночный режим игры (сцена ARDemoScene)
    /// </summary>
    public void OnSingleplayerButtonPress()
    {
        // Загружаю сцену ARDemoScene
        if (!string.IsNullOrEmpty(_singleplayerSceneName))
        {
            SceneLoader.Instance.LoadScene(_singleplayerSceneName);
        }
        else
        {
            Debug.Log("_singleplayerSceneName is null or empty!");
        }
    }

    /// <summary>
    /// Загружает сетевой режим игры Remote Multiplayer (сцена LobbyScene)
    /// </summary>
    public void OnRemoteMultiplayerButtonPress()
    {
        // Загружаю сцену LobbyScene
        
        if (!string.IsNullOrEmpty(_remoteSceneName))
        {
            SceneLoader.Instance.LoadScene(_remoteSceneName);
        }
        else
        {
            Debug.Log("_remoteSceneName is null or empty!");
        }

    }

    /// <summary>
    /// TO DO! Загружает сетевой режим игры Back to Back Multiplayer (сцена ?)
    /// </summary>
    public void OnBackToBackMultiplayerButtonPress()
    {
        // Загружаю сцену TO DO
        

        if (!string.IsNullOrEmpty(_backToBackSceneName))
        {
            //SceneLoader.Instance.LoadScene(_backToBackSceneName);
            Debug.Log("TO DO! Back to Back Multiplayer mode need to be implemented!");
        }
        else
        {
            Debug.Log("_backToBackSceneName is null or empty!");
        }
    }

    #endregion
}
