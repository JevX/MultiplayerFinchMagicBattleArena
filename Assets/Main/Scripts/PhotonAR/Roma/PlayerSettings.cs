// Roma 01.05.2021
// SO Хранит настройки игрока
using UnityEngine;

namespace MAIN.Scripts.GameSettings
{
    [CreateAssetMenu(menuName = "PlayerSettings", fileName = "New Player Settings")]
    public class PlayerSettings : ScriptableObject
    {
        public string playerName = null;
        public Sprite playerAvatarSprite = null;
    }
}
