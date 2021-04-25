using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using MoreMountains.Tools;
using Finch;

[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlaceObject : MMSingleton<ARTapToPlaceObject>, MMEventListener<MMGameEvent>
{   

    [Header("Индикатор места установки")]
    public GameObject spritePlacementIndicator;

    public GameObject spriteWrangDistancePlacement;
    public GameObject spriteRightDistancePlacement;

    public GameObject panelStartAndScale;
    public GameObject dotForRaycast;

    private ARRaycastManager _arRaycastManager;
    private ARPlaneManager _planeManager;
    private GameObject transformValueForTarget; //времянка, принимает изменения трансформа и передает дальше     

    private bool placementPoseIsValid = false; //найдена ли точка для установки модели
    private bool objectIsPlace = false;

    public bool isButtonStartClick = false; //нажата кнопка для старта игры
    public bool isButtonStandClick = false; //нажата кнопка для установки площадки таргетов

    private float koefScale = 0.125f;

    public bool scanningDone = false;

    public bool isCanStand = false;
    public bool isCanMove = true;


    [SerializeField] private float minDistanceToPlayer = 0.05f; //минимальная дистанция, на которую может поставить индикатор игрок
    [SerializeField] private float maxDistanceToPlayer = 4f; //максимальная дистанция, на которую может поставить индикатор игрок

    public LineRenderer debugLine;
    protected override void Awake()
    {
        base.Awake();
        _arRaycastManager = GetComponent<ARRaycastManager>();
        _planeManager = GetComponent<ARPlaneManager>();
    }

    private void Start()
    {
        //spritePlacementIndicator.SetActive(false);
        placementPoseIsValid = false;
        objectIsPlace = false;
        transformValueForTarget = new GameObject();
        
        //spritePlacementIndicator = spriteWrangDistancePlacement;
        isCanStand = false;
        //FinchCalibration.OnCalibrationEnd += CalibrationEndWait;
        CheckDistanceValue();        
    }

    private void ScanningComplete() //сканирование пространства завершено/ можно искать место под площадку таргетов
    {
        scanningDone = true;
    }

    void Update()
    {
        //if (!Finch.FinchCalibration.IsCalibrating)
        if (scanningDone)
        {            
            if (objectIsPlace == false)
            {
                //panelStartAndScale.SetActive(true);
               
                UpdatePlacementIndicator();

                //if (Finch.FinchController.GetPressUp(Finch.Chirality.Any, Finch.RingElement.Touch))
                //{
                //    ButtonClickToStand();
                //}
            }
        }

        if (isButtonStandClick || isButtonStartClick)
        {
            foreach (var plane in _planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }

    }

    /// <summary>
    /// Проверка значений дистанций до игрока, если минимальное большое макимального, то меняет местами
    /// </summary>
    private void CheckDistanceValue()
    {
        if (minDistanceToPlayer > maxDistanceToPlayer)
        {
            var temp = maxDistanceToPlayer;
            maxDistanceToPlayer = minDistanceToPlayer;
            minDistanceToPlayer = temp;
        }
    }

    private void UpdatePlacementIndicator()
    {
        
        var hitsCenter = new List<ARRaycastHit>();      

        Ray raycast = new Ray();

        //raycast.origin = MagicWand.Instance.wandEndPosition.position;
        raycast.origin = MagicWand.Instance.transform.position;
        raycast.direction = MagicWand.Instance.wandEndPosition.forward;

        //raycast.origin = dotForRaycast.transform.position;
        //raycast.direction = dotForRaycast.transform.forward;

        _arRaycastManager.Raycast(raycast, hitsCenter, TrackableType.PlaneWithinPolygon);

        if (hitsCenter.Count > 0)
        {
            debugLine.positionCount = 2;
            debugLine.SetPosition(0, hitsCenter[0].pose.position);
            debugLine.SetPosition(1, raycast.origin);
        }
        else
        {
            debugLine.positionCount = 0;
        }

        placementPoseIsValid = hitsCenter.Count > 0;
        if (placementPoseIsValid)
        { 
            var placePose = hitsCenter[0].pose;

            var cameraForward = Camera.main.transform.forward;//MagicWand.Instance.wandEndPosition.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

            placePose.rotation = Quaternion.LookRotation(cameraBearing);
            
            //spritePlacementIndicator.SetActive(false);
            var distance = Vector3.Distance(placePose.position, Camera.main.transform.position);
            float curKoefScale = 1f;
            isCanStand = false;
            if (distance <= maxDistanceToPlayer && distance >= minDistanceToPlayer)
            {//Right Distance
                spriteWrangDistancePlacement.SetActive(false);
                curKoefScale = koefScale;
                spritePlacementIndicator = spriteRightDistancePlacement;
                isCanStand = true;
            }
            else
            {//Wrang Distance
                spriteRightDistancePlacement.SetActive(false);
                curKoefScale = 0.1f;
                spritePlacementIndicator = spriteWrangDistancePlacement;
                isCanStand = false;
            }
            spritePlacementIndicator.SetActive(true);

            if (isCanMove)
            {
                spritePlacementIndicator.transform.localPosition = new Vector3(placePose.position.x, placePose.position.y, placePose.position.z);
                //var distance = Vector3.Distance(placePose.position, Camera.main.transform.position);

                //if (distance <= maxDistanceToPlayer && distance >= minDistanceToPlayer) 
                //    spritePlacementIndicator.transform.localPosition = new Vector3(placePose.position.x, placePose.position.y, placePose.position.z);

                spritePlacementIndicator.transform.rotation = placePose.rotation;
                spritePlacementIndicator.transform.localScale = Vector3.one * curKoefScale;//PlacementController.Instance.GetKoefScale();
                //Debug.Log("Parent for spritePlacementIndicator = "+ spritePlacementIndicator.transform.parent.name);
            }

            //Debug.Log("placePosition=" + placePose.position + "placeRotation=" + placePose.rotation);
            if (isButtonStandClick && isCanStand) //установили площадку/перестали искать место
            {
                panelStartAndScale.SetActive(false);
                transformValueForTarget.transform.position = placePose.position;
                //-----------
                //var planeManager = GetComponent<ARPlaneManager>();

                //foreach (var plane in _planeManager.trackables)
                //{
                //    plane.gameObject.SetActive(false);
                //    //Debug.Log("Plane Name: " + plane.gameObject.name + "Plane Child Count =" + plane.gameObject.transform.childCount);
                //}
                //-----------
                //PlacementController.Instance.SwitchGroundType();

                //MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.gameStarted);

                objectIsPlace = true;
                
                debugLine.positionCount = 0;
            }            
        }
        else
        {
            spritePlacementIndicator.SetActive(false);
        }

    }   
    
    public void ButtonClickToStart()
    {
        isButtonStartClick = true;
        //MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step4_done);
        TutorialSteps.Instance.Step4_Done();

        TutorialStep5_InfoTarget.Instance.ShowStep5(0); //передается индекс страницы тутора над площадкой
        PlacementController.Instance.SwitchGroundType();
        MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.gameStarted);
    }

    public void SetKoefScale(float value)
    {
        koefScale = value;
    }

    public void ButtonClickToStand()
    {
        isButtonStandClick = true;
        TutorialSteps.Instance.Step3_Done();
        //return isCanStand;
        //MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step3_done);
    }

    public void ResetDataStand()
    {
        TutorialSteps.Instance.Step2_Done();
        //MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.step2_done);
        isButtonStartClick = false;
        isButtonStandClick = false;
        placementPoseIsValid = false;
        objectIsPlace = false;
        transformValueForTarget = new GameObject();
        //PlacementController.Instance.SwitchGroundType();
    }

    
    public void ResetObjectIsPlace()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        switch (gameEvent.EventName)
        {           
            case "Step2_Done":
                ScanningComplete();
                break;
            case "Step3_Done":
               
                break;
            case "Step4_Done":
               
                break;
            case "Step5_Done":
              
                break;
            case "Step6_Done":
               
                break;

            default:
                break;
        }
    }

    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
    }
}