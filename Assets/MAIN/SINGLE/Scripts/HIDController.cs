using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HIDController : MMSingleton<HIDController>
{

    float pressTimer;

    float timeLimit = 0.3f;

    int timesPressed;

    bool canIncrease;

    Coroutine touchCoroutine;

    bool pressed;

    Vector3 lastPos;
    Vector3 currPos;

    Coroutine touchTimer;

    public float TimePressed()
    {
        return pressTimer;
    }

    public int GetTotalPresses()
    {
        return timesPressed;
    }

    public bool OnPressDownTimes(int pressesNeeded)
    {
        return OnPressDown() && timesPressed == pressesNeeded;
    }

    public bool OnPressDown()
    {
        return gameObject.activeInHierarchy && (CheckMouseDown() || CheckTouchDown());
    }


    private bool CheckTouchDown() 
    {
        return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
    }

    private bool CheckMouseDown()
    {
        return Input.GetKeyDown(KeyCode.Mouse0);
    }

    public bool OnPressUp()
    {
        return gameObject.activeInHierarchy && (CheckMouseUp() || CheckTouchUp());
    }

    private bool CheckMouseUp()
    {
        return Input.GetKeyUp(KeyCode.Mouse0);
    }

    private bool CheckTouchUp()
    {
        return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended;
    }

    // Update is called once per frame
    void Update()
    {
        if (OnPressDown())
        {
            pressed = true;
            pressTimer = 0;

            if (touchTimer != null)
                StopCoroutine(touchTimer);
            touchTimer = StartCoroutine(TouchTimer());

            CheckPresses();
        }

        
        if (Input.touchCount == 1)
        {
            currPos = Input.GetTouch(0).position;
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            currPos = Input.mousePosition;
        }

        CalculatePosition(currPos);
    }

    private void LateUpdate()
    {
        if (OnPressUp())
        {
            pressed = false;
        }
    }

    private void CheckPresses()
    {
        if (touchCoroutine != null)
            StopCoroutine(touchCoroutine);
        touchCoroutine = StartCoroutine(TouchCounts());
    }

    IEnumerator TouchCounts()
    {
        timesPressed++;
        yield return new WaitForSeconds(timeLimit);
        timesPressed = 0;
        yield return null;
    }


    IEnumerator TouchTimer()
    {
        while (pressed)
        {
            pressTimer += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    private void CalculatePosition(Vector3 pos)
    {
        float screenLowerPosition = Screen.height * 0.1f;
        float screenUpperPosition = Screen.height - screenLowerPosition;

        if (pos.y < screenLowerPosition)
        {
            float translation = pos.y / screenLowerPosition;
            transform.localRotation = Quaternion.Euler(Mathf.Lerp(-35, 0, translation), 0, 0);
        }
        else if (pos.y > screenUpperPosition)
        {
            float translation = screenUpperPosition / pos.y;
            transform.localRotation = Quaternion.Euler(Mathf.Lerp(35, 0, translation), 0, 0);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        transform.position = Camera.main.ScreenToWorldPoint(pos + Vector3.forward * 0.5f);
    }

}
