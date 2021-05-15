using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAIN.InputSystem
{
    /// я думал это управление для шифтов, но эт чет другое, возможно не нужно, или нужно переделать
    public class InputHIDModule : IUserInputModule
    {
        public bool CheckSwipe()
        {
            throw new System.NotImplementedException();
        }

        public bool GetPressDownTouch()
        {
            return HIDController.Instance.OnPressDownTimes(1);
        }

        public bool GetPressDownHome()
        {
            return HIDController.Instance.OnPressDownTimes(2);
        }

        public bool GetPressUpTouch()
        {
            return HIDController.Instance.OnPressUp();
        }

        public bool GetPressUpHome()
        {
            return HIDController.Instance.OnPressUp();
        }
    }
}