//using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCalibrationFinderIndicator : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField]
    private RectTransform arenaPositionIndicator;

    public GameObject arenaIndicatorLeft;
    public GameObject arenaIndicatorRight;

    ArenaTargetIndicator targetBase = null;


    //public GameObject arenaObject;


    private void Awake()
    {
        //enemyClosestPositionIndicator = transform.Find("EnemyClosest").GetComponent<RectTransform>();
    }
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!TutorialSteps.Instance.isStep1_Complete)
            HandleCalibrationPositionIndicator();
    }

    private void HandleCalibrationPositionIndicator()
    {
        Transform playerCamTransform = mainCamera.transform;

        //Debug.Log("playerCamTransform = " + playerCamTransform.position);
        if (targetBase == null)
        {
            Debug.Log("ind-1-calibr");
            float targetMaxRadius = 100f;
            
            //создает сферу поиска в заданном радиусе, с камерой телефона(игроком) в центре
            Collider[] collider2DArray = Physics.OverlapSphere(playerCamTransform.position, targetMaxRadius);

            foreach (Collider collider in collider2DArray)
            {
                Debug.Log("ind-2-calibr =" + collider2DArray.Length);
                ArenaTargetIndicator arena = collider.GetComponent<ArenaTargetIndicator>();
                if (arena != null)
                {
                    Debug.Log("ind-2-1-calibr");
                    if (targetBase == null)
                    {
                        Debug.Log("ind-2-2-calibr");
                        targetBase = arena;
                    }
                    else
                    {
                        Debug.Log("ind-2-3-calibr");
                        if (Vector3.Distance(playerCamTransform.position, arena.transform.position) < Vector3.Distance(playerCamTransform.position, targetBase.transform.localPosition))
                        {
                            Debug.Log("ind-2-4-calibr");
                            targetBase = arena;
                        }
                    }
                }
            }
        }

        if (targetBase != null)
        {

            Vector3 viewPos = mainCamera.WorldToViewportPoint(targetBase.transform.position);

            if (viewPos.x < 0.05f)
            {
                arenaIndicatorLeft.SetActive(true);
                arenaIndicatorRight.SetActive(false);
            }

            if (viewPos.x > 0.95f)
            {
                arenaIndicatorLeft.SetActive(false);
                arenaIndicatorRight.SetActive(true);
            }

            if (viewPos.x >= 0.05f && viewPos.x <= 0.95f)
            {
                arenaIndicatorLeft.SetActive(false);
                arenaIndicatorRight.SetActive(false);
            }           
        }
        else
        {
            Debug.Log("ind-4-calibr");
            // No enemies alive
            arenaIndicatorLeft.SetActive(false);
            arenaIndicatorRight.SetActive(false);
            arenaPositionIndicator.gameObject.SetActive(false);
        }
    }

    public float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.z, vector.x);
        float degrees = radians * Mathf.Rad2Deg;

        return degrees;
    }
}
