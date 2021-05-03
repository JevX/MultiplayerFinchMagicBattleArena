using UnityEngine;
using UnityEngine.UI;
using MAIN.Scripts.GameSettings;

namespace MAIN.Scripts.UI
{
    public class PlayerSetup : MonoBehaviour
    {
        [Header("Enter Game Button")]
        [SerializeField] private Button _enterGameButton = null;

        [Space(2)]

        [Header("Player Avatars Images")]
        [SerializeField] private Image _playerAvatarImage = null;

        [Space(2)]

        [Header("Player Avatars Selection")]
        [SerializeField] private Button _previousAvatarImageButton = null;
        [SerializeField] private Button _nextAvatarImageButton = null;
        [SerializeField] private Button _selectAvatarImageButton = null;

        private PlayerSettings _playerSettingsSO = null;// Ссылка на SO с настройками игрока
        private PlayerAvatars _playerAvatarsSO = null; // Ссылка на SO с коллекцией аватарок игрока

        private InputField _playerNameInputField = null;

        private int _currentAvatarIndex = 0;

        private void Awake()
        {
            _playerSettingsSO = Resources.Load<PlayerSettings>("ScriptableObjects/PlayerSettings");
            _playerAvatarsSO = Resources.Load<PlayerAvatars>("ScriptableObjects/PlayerAvatars");
        }

        // Start is called before the first frame update
        private void Start()
        {
            _playerNameInputField = FindObjectOfType<InputField>();

            // Adding Button listeners
            _enterGameButton.onClick.AddListener(OnEnterGameButtonPress);

            _nextAvatarImageButton.onClick.AddListener(NextAvatarImage);
            _previousAvatarImageButton.onClick.AddListener(PreviousAvatarImage);
            _selectAvatarImageButton.onClick.AddListener(SelectAvatarImage);

            if (_playerAvatarsSO != null && _playerAvatarsSO.PlayerAvatarsCollection != null)
            {
                _playerAvatarImage.sprite = _playerAvatarsSO.PlayerAvatarsCollection[_currentAvatarIndex];
            }
        }

        private void OnDestroy()
        {
            // Removing Button listeners
            _enterGameButton.onClick.RemoveListener(OnEnterGameButtonPress);

            _nextAvatarImageButton.onClick.RemoveListener(NextAvatarImage);
            _previousAvatarImageButton.onClick.RemoveListener(PreviousAvatarImage);
            _selectAvatarImageButton.onClick.AddListener(SelectAvatarImage);
        }

        #region UNITY UI Methods
        /// <summary>
        /// Сохраняет выбранное имя игрока в SO PlayerSettings
        /// </summary>
        private void OnEnterGameButtonPress()
        {
            // Просто выбираем и сохраняем в SO
            string playerName = _playerNameInputField.text;

            if (!string.IsNullOrEmpty(playerName))
            {
                _playerSettingsSO.playerName = playerName;
            }
            else
            {
                Debug.Log("Player name is invalid or empty!");
            }
        }

        /// <summary>
        /// Переключает на следующий доступный аватар
        /// </summary>
        private void NextAvatarImage()
        {
            if (_playerAvatarsSO != null && _playerAvatarsSO.PlayerAvatarsCollection != null)
            {
                if (_currentAvatarIndex + 1 <= _playerAvatarsSO.PlayerAvatarsCollection.Length - 1)
                {
                    _currentAvatarIndex++;
                    _playerAvatarImage.sprite = _playerAvatarsSO.PlayerAvatarsCollection[_currentAvatarIndex];
                }
                else
                {
                    _currentAvatarIndex = 0;
                    _playerAvatarImage.sprite = _playerAvatarsSO.PlayerAvatarsCollection[_currentAvatarIndex];
                }
            }
            else
            {
                Debug.Log("_playerAvatarsSO or _playerAvatarsSO.PlayerAvatarsCollection is null!");
            }
        }

        /// <summary>
        /// Переключает на предыдущий доступный аватар
        /// </summary>
        private void PreviousAvatarImage()
        {
            if (_playerAvatarsSO != null && _playerAvatarsSO.PlayerAvatarsCollection != null)
            {
                if (_currentAvatarIndex - 1 >= 0)
                {
                    _currentAvatarIndex--;
                    _playerAvatarImage.sprite = _playerAvatarsSO.PlayerAvatarsCollection[_currentAvatarIndex];
                }
                else
                {
                    _currentAvatarIndex = _playerAvatarsSO.PlayerAvatarsCollection.Length - 1;
                    _playerAvatarImage.sprite = _playerAvatarsSO.PlayerAvatarsCollection[_currentAvatarIndex];
                }
            }
            else
            {
                Debug.Log("_playerAvatarsSO or _playerAvatarsSO.PlayerAvatarsCollection is null!");
            }
        }

        /// <summary>
        /// Сохраняет выбранный аватар в SO PlayerSettings
        /// </summary>
        private void SelectAvatarImage()
        {
            // Просто выбираем и сохраняем в SO
            if (_playerSettingsSO != null)
            {
                _playerSettingsSO.playerAvatarSprite = _playerAvatarImage.sprite;
                _playerSettingsSO.playerAvatarSpriteIndex = _currentAvatarIndex;
            }
            else
            {
                Debug.Log("_playerSettingsSO is null!");
            }
          
        }
        #endregion
    }
}
