using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAIN.InputSystem
{
    // модуль управления, набор методов основан на устройстве FinchRing, действие которых иммитируется с других устройств
    public interface IUserInputModule
    {
        bool CheckSwipe();
        bool GetPressDownTouch();
        bool GetPressDownHome();
        bool GetPressUpTouch();
        bool GetPressUpHome();
    }
}