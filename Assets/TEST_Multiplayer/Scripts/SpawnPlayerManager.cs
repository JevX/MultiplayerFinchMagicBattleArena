using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace MAIN.Spawn
{
    public class SpawnPlayerManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject playerPrefab;

        [Header("Площадка, которую игрок размещает в AR")]
        [SerializeField] private GameObject mainObject;

        [Header("Камера-родитель игрока")]
        [SerializeField] private Transform cameraAR;
        
        [Header("начальные Координаты создания игрока")]
        [SerializeField] private Vector3 instantiatePosition = Vector3.zero; 

        [Header("Создание игрока относительно камеры")]
        [SerializeField] private Vector3 localPosition = Vector3.zero; 
        
        private GameObject playerGameobject2 =null;

        // PunCallback
        public override void OnJoinedRoom()
        {
            Debug.Log($"присоеденен к комнате {PhotonNetwork.CurrentRoom.Name}  {PhotonNetwork.CurrentRoom.Players.Count}");
            if (PhotonNetwork.IsConnectedAndReady)
            {
                SpawnPlayer();
            }
        }

        private void SpawnPlayer()
        {
            // создаем для обоих, но камеру делаем родителем только для текущего
            playerGameobject2 = PhotonNetwork.Instantiate("PLAYER", instantiatePosition, Quaternion.identity);
            playerGameobject2.transform.SetParent(cameraAR);
            playerGameobject2.transform.localPosition = localPosition;
        }
    }
}
