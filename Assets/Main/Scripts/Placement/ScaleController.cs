// Modifyed by Roman Baranov 07.05.2021

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class ScaleController : MonoBehaviour
{
    #region VARIABLES
    private ARSessionOrigin m_ARSessionOrigin = null;

    [Header("AR Origin Scale Slider")]
    [SerializeField] private Slider _aROriginScaleSlider = null;
    #endregion

    #region UNITY Methods
    private void Awake()
    {
        m_ARSessionOrigin = GetComponent<ARSessionOrigin>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _aROriginScaleSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnDestroy()
    {
        _aROriginScaleSlider?.onValueChanged.RemoveAllListeners();
    }
    #endregion

    #region PRIVATE Methods
    /// <summary>
    /// Масштабирует AR Origin относительно сцены.
    /// </summary>
    /// <param name="value">Входящее значение масштаба. Больше значение, меньше окружение на сцене</param>
    private void OnSliderValueChanged(float value)
    {
        if (_aROriginScaleSlider != null)
        {
            m_ARSessionOrigin.transform.localScale = Vector3.one / value;
        }
    }
    #endregion
}
