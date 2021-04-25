
using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Scripts.PhotonAR
{
    public class PhotonNetworkConnector : MonoBehaviourPunCallbacks
    {
        [SerializeField] private float waitToConnect = 10;
        [Header("Окно выбора сцены")] 
        [SerializeField] private GameObject SelectPanel = null;
        
        [Header("Окно Загрузки")] 
        [SerializeField] private GameObject LoadPanel = null;
        [SerializeField] private Text loadText = null;
        
        [Header("Кнопки перехода на другую сцену")]
        [SerializeField] private Button btnConnectLocal = null;
        [SerializeField] private string nameSceneLocal = null;
        [SerializeField] private Button btnConnectNetwork = null;
        [SerializeField] private string nameConnectNetwork = null;
        
        private string selectNextSceneName = null;
        
        private void Start()
        {
            btnConnectLocal.onClick.AddListener(() => { OnConnectPhotonNetwork(nameSceneLocal);});
            btnConnectNetwork.onClick.AddListener(() => { OnConnectPhotonNetwork(nameConnectNetwork);});
        }


        private void OnDestroy()
        {
            btnConnectLocal.onClick.RemoveAllListeners();
            btnConnectNetwork.onClick.RemoveAllListeners();
        }

        private void OnConnectPhotonNetwork(string nextScene)
        {
            selectNextSceneName = nextScene;
            if (!PhotonNetwork.IsConnected) StartCoroutine(ConnectionToPhoton());
            else SceneLoadManager.Instance.LoadScene(selectNextSceneName);
        }
    
        public override void OnConnected()
        {
            Debug.Log("Мы подключены к фотону");
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log(PhotonNetwork.LocalPlayer.NickName + " подключен к серверу");
            
            SceneLoadManager.Instance.LoadScene(selectNextSceneName);
        }

        private void SetActiveLoadPanel(bool isActive)
        {
            SelectPanel.SetActive(!isActive);
            LoadPanel.SetActive(isActive);
        }
        
        private IEnumerator ConnectionToPhoton()
        {
            SetActiveLoadPanel(true);
            float currentWait = 0;
            PhotonNetwork.GameVersion = Application.version; //TODO возможно убрать 
            PhotonNetwork.ConnectUsingSettings();
            while (currentWait < waitToConnect)
            {
                if (PhotonNetwork.IsConnected) break;
                loadText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
                currentWait += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            if (!PhotonNetwork.IsConnected)  SetActiveLoadPanel(false); // TODO возможно добавить сообщение об ошибке
        }

    }
}
