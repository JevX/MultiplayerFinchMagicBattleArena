// Roma 02.05.2021
// SO Хранит коллекцию аватарок для игрока

using UnityEngine;

[CreateAssetMenu(menuName = "PlayerAvatars", fileName = "NewAvatarsCollection")]
public class PlayerAvatars : ScriptableObject
{
    #region VARIABLES
    [SerializeField] private Sprite[] _playerAvatarsCollection = null;
    public Sprite[] PlayerAvatarsCollection { get { return _playerAvatarsCollection; } }
    #endregion
}
