using Photon.Pun;
using UnityEngine;

namespace Wand
{
    //тестовый спавн палочки, нужно будет интегрировать в какойнибудь spawn player manager вместе со спавном игрока
    public class SpawnerMagicWand : MonoBehaviourPunCallbacks
    {
        // скорее всего это нужно будет переделать под устройство финча, я на данный момент не совсем понимаю как оно работает фактически
        [Header("родитель палочки")]
        [SerializeField] private Transform parentForWandController = null;
        [Header("Позиция относительно камеры")]
        [SerializeField] private Vector3 posSpawnWand = Vector3.forward;
        
        [Header("Префаб модели для отображения других игроков")]
        [SerializeField] private GameObject wandPhotonPrefab = null;
        [Header("Префаб логики для текущего игрока")]
        [SerializeField] private GameObject wandMainPrefab = null;

        public override void OnJoinedRoom()
        {
            // создаём модель для всех игроков в комнате
            GameObject wand = PhotonNetwork.Instantiate(wandPhotonPrefab.name, posSpawnWand, Quaternion.identity);
            GameObject wandController = null;
            
            // для текущего игрока добавляем логику к палочке и правильно устанавливаем её положение
            PhotonView view = wand.GetComponent<PhotonView>();
            if (view.IsMine)
            {
                wandController = Instantiate(wandMainPrefab, parentForWandController, true);
                wand.transform.SetParent(wandController.transform);
                wandController.transform.localPosition = posSpawnWand;
                wand.transform.localPosition = Vector3.zero;
                wand.transform.localRotation = Quaternion.identity;
                wandController.transform.localRotation = Quaternion.identity;
            }
        }
    }
}
