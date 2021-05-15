using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAIN.InputSystem
{
    /// модуль для компьютера
    public class InputPCModule : IUserInputModule
    {
        public bool CheckSwipe()
        {
            return Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0 && !Input.GetMouseButton(0) && !Input.GetMouseButton(1);
        }

        public bool GetPressDownTouch()
        {
            return Input.GetMouseButtonDown(0);
        }

        public bool GetPressDownHome()
        {
            return Input.GetMouseButtonDown(1);
        }

        public bool GetPressUpTouch()
        {
            return Input.GetMouseButtonUp(0);
        }

        public bool GetPressUpHome()
        {
            return Input.GetMouseButtonUp(1);
        }
    }
}