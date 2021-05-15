using UnityEngine;

namespace MAIN.InputSystem
{
    // Система управления - создаем модуль по заранее выбранному типу, можно сделать возможность выбора в самой игре
    public class UserInputController : MonoBehaviour, IUserInputModule // TODO убрать интерфейс
    {
        public static UserInputController Instance = null;
        
        /// тип управления
        [SerializeField] private InputType inputType;
        
        private IUserInputModule _module = null;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            /// устанавливаем модуль
            switch (inputType)
            {
                default:
                case InputType.PC:
                    _module = new InputPCModule();
                    break;
                case InputType.HID:
                    _module = new InputHIDModule();
                    break;
                case InputType.FinchRing:
                    _module = new InputFinchRingModule();
                    break;
            }
        }

        public bool CheckSwipe()
        {
            return _module.CheckSwipe();
        }

        public bool GetPressDownTouch()
        {
            return _module.GetPressDownTouch();
        }

        public bool GetPressDownHome()
        {
            return _module.GetPressDownHome();
        }

        public bool GetPressUpTouch()
        {
            return _module.GetPressUpTouch();
        }

        public bool GetPressUpHome()
        {
            return _module.GetPressUpHome();
        }
    }

    public enum InputType
    {
        PC,
        HID,
        FinchRing
    }
}