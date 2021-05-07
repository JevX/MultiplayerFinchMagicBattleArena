// Modifyed by Roman Baranov 07.05.2021

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARPlacementAndPlaneDetectionController : MonoBehaviour
{
    #region VARIABLES
    private ARPlaneManager _aRPlaneManager = null;
    private ARPlacementManager _aRPlacementManager = null;

    [Header("Land Placement Buttons")]
    [SerializeField] private Button _placeButton = null;
    [SerializeField] private Button _redeployButton = null;
    [SerializeField] private Button _searchGamesButton = null;

    [Space(2)]

    [Header("AR Origin Scale Slider")]
    [SerializeField] private Slider _aROriginScaleSlider = null;

    [Space(2)]

    [Header("Connection Inform Popup")]
    [SerializeField] private Text _connectionInformPopupText = null;
    #endregion

    #region UNITY Methods
    private void Awake()
    {
        _aRPlaneManager = GetComponent<ARPlaneManager>();
        _aRPlacementManager = GetComponent<ARPlacementManager>();

        _placeButton.onClick.AddListener(DisableARPlacementAndPlaneDetection);
        _redeployButton.onClick.AddListener(EnableARPlacementAndPlaneDetection);
    }

    // Start is called before the first frame update
    private void Start()
    {
        _placeButton.enabled = true;
        _aROriginScaleSlider.enabled = true;

        _redeployButton.enabled = false;
        _searchGamesButton.enabled = false;
    }
    private void OnDestroy()
    {
        _placeButton?.onClick.RemoveAllListeners();
        _redeployButton?.onClick.RemoveAllListeners();
        _searchGamesButton?.onClick.RemoveAllListeners();
    }
    #endregion

    #region PRIVATE Methods
    /// <summary>
    /// Отключает определение поверхностей в режиме AR
    /// </summary>
    private void DisableARPlacementAndPlaneDetection()
    {
        _aRPlaneManager.enabled = false;
        _aRPlacementManager.enabled = false;
        SetAllPlanesActiveOrDeactive(false);

        _aROriginScaleSlider.enabled = false;

        _placeButton.enabled = false;
        _redeployButton.enabled = true;
        _searchGamesButton.enabled = true;

        _connectionInformPopupText.text = "Great! You placed the ARENA..Now, search for games to BATTLE!";
    }

    /// <summary>
    /// Включает определение поверхностей в режиме AR
    /// </summary>
    private void EnableARPlacementAndPlaneDetection()
    {
        _aRPlaneManager.enabled = true;
        _aRPlacementManager.enabled = true;
        SetAllPlanesActiveOrDeactive(true);
        _aROriginScaleSlider.enabled = true;

        _placeButton.enabled = true;
        _redeployButton.enabled = false;
        _searchGamesButton.enabled = false;

        _connectionInformPopupText.text = "Move phone to detect planes and place the Battle Arena!";  
    }

    /// <summary>
    /// Переключает режим определения поверхностей в режиме AR 
    /// </summary>
    /// <param name="value">Целевое состояние режима определения поверхностей</param>
    private void SetAllPlanesActiveOrDeactive(bool value)
    {
        foreach (var plane in _aRPlaneManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
    }
    #endregion
}
