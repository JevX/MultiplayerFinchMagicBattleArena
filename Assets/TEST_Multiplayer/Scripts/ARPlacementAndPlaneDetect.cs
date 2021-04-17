using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARPlacementAndPlaneDetect : MonoBehaviour
{
    ARPlaneManager m_ARPlaneManager;
    ARPlacementManager m_ARPlacementManager;

    public Button placeButton;
    public GameObject scaleSlider;
    
    private void Awake()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
        m_ARPlacementManager = GetComponent<ARPlacementManager>();
    }
    
    void Start()
    {
        placeButton.gameObject.SetActive(true);
        scaleSlider.SetActive(true);
        EnableARPlacementAndPlaneDetection();
        placeButton.onClick.AddListener(DisableARPlacementAndPlaneDetection);
    }

    public void DisableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = false;
        m_ARPlacementManager.enabled = false;
        SetAllPlanesActiveOrDeactive(false);

        scaleSlider.SetActive(false);

        placeButton.gameObject.SetActive(false);

    }

    public void EnableARPlacementAndPlaneDetection()
    {
        m_ARPlaneManager.enabled = true;
        m_ARPlacementManager.enabled = true;
        SetAllPlanesActiveOrDeactive(true);
        scaleSlider.SetActive(true);

        placeButton.gameObject.SetActive(true);

    }

    private void SetAllPlanesActiveOrDeactive(bool value)
    {
        foreach (var plane in m_ARPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }
}
