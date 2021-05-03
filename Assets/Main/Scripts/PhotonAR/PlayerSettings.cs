// Roma 01.05.2021
// SO Хранит настройки игрока
using UnityEngine;

namespace MAIN.Scripts.GameSettings
{
    [CreateAssetMenu(menuName = "PlayerSettings", fileName = "New Player Settings")]
    public class PlayerSettings : ScriptableObject
    {
        public string playerName = null;// Имя игрока

        public Sprite playerAvatarSprite = null;// Спрайт для выбранной аватарки игрока
        public int playerAvatarSpriteIndex = 0;// Индекс для выбранной аватарки игрока
    }
}
