using Finch;

namespace MAIN.InputSystem
{
    /// модуль для кольца, на его методах основаны остальные
    public class InputFinchRingModule : IUserInputModule
    {
    
        public bool CheckSwipe()
        {
            return FinchInput.GetTouchpadEvent(NodeType.LeftHand, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeLeft) ||
                   FinchInput.GetTouchpadEvent(NodeType.LeftHand, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeRight) ||
                   FinchInput.GetTouchpadEvent(NodeType.RightHand, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeLeft) ||
                   FinchInput.GetTouchpadEvent(NodeType.RightHand, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeRight);
        }

        public bool GetPressDownTouch()
        {
            return FinchController.GetPressDown(Chirality.Any, RingElement.Touch);
        }
    
        public bool GetPressDownHome()
        {
            return FinchController.GetPressDown(Chirality.Any, RingElement.HomeButton);
        }

        public bool GetPressUpTouch()
        {
            return FinchController.GetPressUp(Chirality.Any, RingElement.Touch);
        }
    
        public bool GetPressUpHome()
        {
            return FinchController.GetPressUp(Chirality.Any, RingElement.HomeButton);
        }
    
    }
}
