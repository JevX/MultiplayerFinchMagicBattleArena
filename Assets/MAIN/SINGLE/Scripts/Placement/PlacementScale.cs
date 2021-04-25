using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Finch;

public class PlacementScale : MonoBehaviour
{
    public Slider sliderScale;

    NodeType Node;

    // Update is called once per frame
    void Update()
    {
        if (TutorialSteps.Instance.isStep3_Complete && !TutorialSteps.Instance.isStep4_Complete)
        {
            //Debug.Log("Into Slider");
            //to the right
            if (Finch.FinchInput.GetTouchpadEvent(Node, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeLeft))
            {
                //Debug.Log("Into Slider - SwipeLeft");
                sliderScale.value -= 0.015f;
            }

            //to the Left
            if (Finch.FinchInput.GetTouchpadEvent(Node, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeRight))
            {
                //Debug.Log("Into Slider - SwipeRight");
                sliderScale.value += 0.015f;                
            }
        }
    }
}
