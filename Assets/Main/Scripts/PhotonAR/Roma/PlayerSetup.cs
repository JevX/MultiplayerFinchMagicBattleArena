using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;

namespace MAIN.Scripts.UI
{
    public class PlayerSetup : MonoBehaviour
    {
        [Header("Enter Game Button")]
        [SerializeField] private Button _enterGameButton = null;

        [Space(2)]

        [Header("Player Avatars Images")]
        [SerializeField] private Image _playerAvatarImage = null;
        [SerializeField] private Sprite[] _playerAvatars = null;

        [Space(2)]

        [Header("Player Avatars Selection")]
        [SerializeField] private Button _previousAvatarImageButton = null;
        [SerializeField] private Button _nextAvatarImageButton = null;
        [SerializeField] private Button _selectAvatarImageButton = null;

        private InputField _playerNameInputField = null;

        private int _currentAvatarIndex = 0;

        private void Awake()
        {
            
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

            if (_playerAvatars != null)
            {
                _playerAvatarImage.sprite = _playerAvatars[_currentAvatarIndex];
            }

            Debug.Log($"_currentAvatarIndex {_currentAvatarIndex}");
            Debug.Log($"_playerAvatars.Length {_playerAvatars.Length}");
            Debug.Log($"_playerAvatarImage.sprite.name {_playerAvatarImage.sprite.name}");
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
        /// TO DO Сохраняет выбранное имя игрока в Scriptable Object
        /// </summary>
        private void OnEnterGameButtonPress()
        {
            // Просто выбираем и сохраняем в SO
            string playerName = _playerNameInputField.text;

            if (!string.IsNullOrEmpty(playerName))
            {
                if (!PhotonNetwork.IsConnected)
                {
                    PhotonNetwork.LocalPlayer.NickName = playerName;
                }
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
            if (_playerAvatars != null)
            {
                if (_currentAvatarIndex + 1 <= _playerAvatars.Length - 1)
                {
                    _currentAvatarIndex++;
                    _playerAvatarImage.sprite = _playerAvatars[_currentAvatarIndex];
                }
                else
                {
                    _currentAvatarIndex = 0;
                    _playerAvatarImage.sprite = _playerAvatars[_currentAvatarIndex];
                }
            }
        }

        /// <summary>
        /// Переключает на предыдущий доступный аватар
        /// </summary>
        private void PreviousAvatarImage()
        {
            if (_playerAvatars != null)
            {
                if (_currentAvatarIndex - 1 >= 0)
                {
                    _currentAvatarIndex--;
                    _playerAvatarImage.sprite = _playerAvatars[_currentAvatarIndex];
                }
                else
                {
                    _currentAvatarIndex = _playerAvatars.Length - 1;
                    _playerAvatarImage.sprite = _playerAvatars[_currentAvatarIndex];
                }
            }
        }

        /// <summary>
        /// TO DO Сохраняет выбранный аватар в Scriptable Object
        /// </summary>
        private void SelectAvatarImage()
        {
            // Просто выбираем и сохраняем в SO
            //Hashtable hashtable = new Hashtable();
            //PhotonNetwork.LocalPlayer.
        }
        #endregion
    }
}
