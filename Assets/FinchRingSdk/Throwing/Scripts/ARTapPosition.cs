using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class ARTapPosition : MonoBehaviour
{
    public GameObject Instance;
    private GameObject spawnedObject;
    private ARRaycastManager _arRCmanager;
    private Vector2 touchpoint;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        _arRCmanager = GetComponent<ARRaycastManager>();

    }

    bool TryGetTocuhPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    void Update()
    {
        if (!TryGetTocuhPosition(out Vector2 touchPosition))
            return;

        if (_arRCmanager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;


            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(Instance, hitPose.position, hitPose.rotation);
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
            }
        }
    }
}
