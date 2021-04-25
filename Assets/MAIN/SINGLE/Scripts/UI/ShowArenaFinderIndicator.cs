//using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowArenaFinderIndicator : MonoBehaviour
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
        if (TutorialSteps.Instance.isStep3_Complete)
            HandleArenaPositionIndicator();
    }

    private void HandleArenaPositionIndicator()
    {
        Transform playerCamTransform = mainCamera.transform;

        //Debug.Log("playerCamTransform = " + playerCamTransform.position);
        if (targetBase == null)
        {
            Debug.Log("ind-1");
            float targetMaxRadius = 100f;
            
            //создает сферу поиска в заданном радиусе, с камерой телефона(игроком) в центре
            Collider[] collider2DArray = Physics.OverlapSphere(playerCamTransform.position, targetMaxRadius);

            foreach (Collider collider in collider2DArray)
            {
                Debug.Log("ind-2 =" + collider2DArray.Length);
                ArenaTargetIndicator arena = collider.GetComponent<ArenaTargetIndicator>();
                if (arena != null)
                {
                    Debug.Log("ind-2-1");
                    if (targetBase == null)
                    {
                        Debug.Log("ind-2-2");
                        targetBase = arena;
                    }
                    else
                    {
                        Debug.Log("ind-2-3");
                        if (Vector3.Distance(playerCamTransform.position, arena.transform.position) < Vector3.Distance(playerCamTransform.position, targetBase.transform.localPosition))
                        {
                            Debug.Log("ind-2-4");
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

            //Vector3 dirToClosestEnemy = (targetBase.transform.localPosition - playerCamTransform.forward).normalized;
            //Debug.Log("ind-3");
            ////arenaPositionIndicator.anchoredPosition = dirToClosestEnemy * 25f; // было 25
            //float angle = GetAngleFromVector(dirToClosestEnemy);
            //Debug.Log("ind-3-1(angle) =" + angle + " cam angle ="+playerCamTransform.rotation);
            //if (angle > 30)
            //{
            //    arenaIndicatorLeft.SetActive(true);
            //    arenaIndicatorRight.SetActive(false);
            //}
            //else if (angle < -30)
            //{
            //    arenaIndicatorLeft.SetActive(false);
            //    arenaIndicatorRight.SetActive(true);
            //}
            //else
            //{
            //    arenaIndicatorLeft.SetActive(false);
            //    arenaIndicatorRight.SetActive(false);
            //}

            //arenaPositionIndicator.eulerAngles = new Vector3(0, 0, angle);// GetAngleFromVector(dirToClosestEnemy)*2f);
            //Debug.Log("ind-3-1(angle) ="+angle);
            //float distanceToClosestEnemy = Vector3.Distance(targetBase.transform.position, playerCamTransform.forward);
            //arenaPositionIndicator.gameObject.SetActive(distanceToClosestEnemy > 2f); //было 1,5
            //arenaPositionIndicator.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("ind-4");
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
