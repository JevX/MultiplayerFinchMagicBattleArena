//24.04.2021 Roman Baranov
using UnityEngine;


namespace MAIN.Scripts.UI
{
    public class GameModeLoader : MonoBehaviour
    {
        [Header("Singleplayer Scene Name")]
        [SerializeField] private string _singleplayerSceneName = null;

        [Space(2)]

        [Header("Multiplayer Scenes Names")]
        [SerializeField] private string _backToBackSceneName = null;
        [SerializeField] private string _remoteSceneName = null;

        private void Awake()
        {
            UIInterfaceController.Instance.OpenInterface(InterfaceType.GameSelection_PlayerLoginPopup);
        }

        #region UI Callback Methods
        /// <summary>
        /// Загружает одиночный режим игры (сцена ARDemoScene)
        /// </summary>
        public void OnSingleplayerButtonPress()
        {
            // Загружаю сцену 
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
        /// Загружает сетевой режим игры Remote Multiplayer
        /// </summary>
        public void OnRemoteMultiplayerButtonPress()
        {
            // Загружаю сцену

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
        /// TO DO! Загружает сетевой режим игры Back to Back Multiplayer
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
}
