// Modifyed by Roman Baranov 07.05.2021

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementManager : MonoBehaviour
{
    #region VARIABLES
    private ARRaycastManager m_ARRaycastManager = null;
    static List<ARRaycastHit> raycast_Hits = new List<ARRaycastHit>();

    [SerializeField] private Camera _aRCamera = null;

    [SerializeField] private GameObject _placementLand = null;
    #endregion

    #region UNITY Methods
    private void Awake()
    {
        m_ARRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 centerOfScreen = new Vector3(Screen.width / 2, Screen.height / 2);
        Ray ray = _aRCamera.ScreenPointToRay(centerOfScreen);

        if (m_ARRaycastManager.Raycast(ray,raycast_Hits,TrackableType.PlaneWithinPolygon))
        {
            //Intersection!
            Pose hitPose = raycast_Hits[0].pose;

            Vector3 positionToBePlaced = hitPose.position;

            _placementLand.transform.position = positionToBePlaced;
        }
    }
    #endregion
}
