using Photon.Pun;
using UnityEngine;

namespace Main.Scripts.PhotonAR
{
    public class PhotonSpawnPlayer : MonoBehaviourPunCallbacks
    {
        [Header("Префаб игрока")]
        [SerializeField] private GameObject playerPrefab = null;
        
        [Header("Якоря")]
        [SerializeField] private GameObject objectAnchor = null;
        [SerializeField] private Transform cameraAR;
        
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
            Vector3 instantiatePosition = Vector3.zero; //TODO
            GameObject playerGO = PhotonNetwork.Instantiate(playerPrefab.name, instantiatePosition, Quaternion.identity);
            playerGO.transform.SetParent(cameraAR);
        }
    }
}