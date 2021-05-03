// Roma 01.05.2021
// SO Хранит настройки игрока
using UnityEngine;

namespace MAIN.Scripts.GameSettings
{
    [CreateAssetMenu(menuName = "PlayerSettings", fileName = "NewPlayerSettings")]
    public class PlayerSettings : ScriptableObject
    {
        #region VARIABLES
        public string playerName = null;// Имя игрока

        public Sprite playerAvatarSprite = null;// Спрайт для выбранной аватарки игрока
        public int playerAvatarSpriteIndex = 0;// Индекс для выбранной аватарки игрока
        #endregion
    }
}
