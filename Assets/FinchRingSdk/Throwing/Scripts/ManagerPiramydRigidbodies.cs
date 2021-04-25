using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerPiramydRigidbodies : MonoBehaviour
{
    private CenterFigureAnimator centerFigureAnimation;

    private float timerThrowedObjectInFlight = float.MinValue;

    private bool bBeenThrowed = true;

    [HideInInspector]
    public ThrowingObject PickapedObject = null;
    private bool deletedWhenCalibrated = false;

    public bool b_need_disable_collider_in_hand = true;

    public int MaxThrowingObject = 20;

    [HideInInspector]
    public List<ThrowingObject> throwingObjects = new List<ThrowingObject>();
    //private bool 
    // Use this for initialization
    void Start()
    {
        DetectAnimator();
    }

    void DetectAnimator()
    {
        if (centerFigureAnimation == null)
            centerFigureAnimation = GameObject.Find("Object")?.GetComponent<CenterFigureAnimator>();
    }


    ///контролирование количества обьектов 
    void ThrowingObjectCountControl()
    {
        if (throwingObjects.Count > MaxThrowingObject)
        {
            throwingObjects[0].DestroyObject();
        }
    }

    // Update is called once per frame
    void Update()
    {
        DetectAnimator();

        if (bBeenThrowed)
        {
            timerThrowedObjectInFlight += Time.deltaTime;

            // Spawn new object when it in flight more 
            // then several seconds and not collide with anything
            if (timerThrowedObjectInFlight > 3
                && timerThrowedObjectInFlight != float.MinValue
                && !Finch.FinchCalibration.IsCalibrating)
            {
                timerThrowedObjectInFlight = float.MinValue;

                Spawn();
            }
        }


        //destroy current object when calibrating is run
        //but he has been created
        if (Finch.FinchCalibration.IsCalibrating && !deletedWhenCalibrated)
        {
            if (centerFigureAnimation?.currentSpawnedObject != null && !CenterFigureAnimator.ReitaergrationAnimationPlay)
            {
                //ActivateThrwed(centerFigureAnimation.currentSpawnedObject.GetComponent<trowedObj>());
                Destroy(centerFigureAnimation.currentSpawnedObject);
                deletedWhenCalibrated = true;
                centerFigureAnimation.HideHologram();

            }

        }
        else if (!Finch.FinchCalibration.IsCalibrating && deletedWhenCalibrated)
        {
            Spawn();
            deletedWhenCalibrated = false;
        }

        ThrowingObjectCountControl();
    }

    public bool IsPickUped()
    {
        return PickapedObject != null;
    }

    private void ActivateThrowed(ThrowingObject obj)
    {
        // Update PickapedObject to flight

        if (b_need_disable_collider_in_hand)
            obj.GetComponent<MeshCollider>().enabled = true;//enable collider after throw

        obj.readyToOptimize = true;
        obj.SwitchColliderToNormal();
        obj.CreateAndConnectTrail();

        // fall down 
        obj.GetComponent<Rigidbody>().useGravity = true;
        // unparent it to be sure
        obj.transform.parent = null;
    }


    public void ThrowObject()
    {
        if (PickapedObject == null)
            Debug.LogError("ThrowObject is imposible, it should be picked up already");

        //print("ThrowObject");

        ActivateThrowed(PickapedObject);

        timerThrowedObjectInFlight = 0;

        // picked been throwed
        bBeenThrowed = true;
        PickapedObject = null;
    }


    public void PickUp(GameObject pickapedObject)
    {
        if (pickapedObject == null)
        {
            print("try to handle null pickuped object");
            pickapedObject = centerFigureAnimation.currentSpawnedObject;

            if (pickapedObject == null)
                return;

            print("hadel null pickuped objected");
            ThrowObject();
            bBeenThrowed = false;
            return;
        }

        // print("PickUp");

        if (!bBeenThrowed)
        {
            Debug.LogWarning("Pickup is imposible, you should throw it before");
            return;
        }

        if (b_need_disable_collider_in_hand)
            pickapedObject.GetComponent<MeshCollider>().enabled = false;//disable collider after pick up

        PickapedObject = pickapedObject.GetComponent<ThrowingObject>();
        bBeenThrowed = false;
        if (pickapedObject == centerFigureAnimation.currentSpawnedObject)
            centerFigureAnimation.currentSpawnedObject = null;
    }



    private void Spawn()
    {
        // do not spawn when pyramid already spawn
        // or animation reintegration play
        // or calibratin has run
        if (centerFigureAnimation.currentSpawnedObject != null ||
            CenterFigureAnimator.ReitaergrationAnimationPlay || Finch.FinchCalibration.IsCalibrating)/// <--------------
            return;

        centerFigureAnimation.RunAnimation_Reintegration();
    }


    public void CollideWithFloor()
    {
        // print("CollideWithFloor");
        if (PickapedObject != null)
            return;
        Spawn();
    }

    public void MakeCurentObjectPhysical()
    {
        GameObject current = centerFigureAnimation.currentSpawnedObject;

        ActivateThrowed(current.GetComponent<ThrowingObject>());

        centerFigureAnimation.currentSpawnedObject = null;
        Spawn();
    }

}
