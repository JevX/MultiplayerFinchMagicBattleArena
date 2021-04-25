using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;

public class PlacementController : MMSingleton<PlacementController>
{    
    public GameObject groundIndicatorEmpty; //летает как индикатор, отключается при постановке на поверхность
    public GameObject groundWithGameObjects;//включает объекты игры

    public Slider sliderForScaleIndictor; //ползунок масштабирования Индикатора
    public float koefScale = 1f; //значение масштаба Индикатора

    [SerializeField] private float minScaleIndicator = 0.05f; //минимальный значение масштаба Индикатора
    [SerializeField] private float midiScaleIndicator = 0.125f; //серединное значение масштаба Индикатора
    [SerializeField] private float maxScaleIndicator = 0.2f; //максимальное значение масштаба Индикатора

    //[SerializeField] private float minDistanceToPlayer = 1f; //минимальная дистанция, на которую может поставить индикатор игрок
    //[SerializeField] private float maxDistanceToPlayer = 3f; //максимальная дистанция, на которую может поставить индикатор игрок

    private bool isGroundIndicatorStatus = false; //статус включенности Индикатора

    public List<GameObject> listOfDots; //точки для спавна Таргетов на Индикаторе    

    private void Start()
    {        
        sliderForScaleIndictor.minValue = minScaleIndicator;
        sliderForScaleIndictor.maxValue = maxScaleIndicator;        

        sliderForScaleIndictor.value = midiScaleIndicator;//по умолчанию устанавливается среднее значение        //minScaleIndicator;

        koefScale = midiScaleIndicator;

        isGroundIndicatorStatus = true;

        //if (TargetsManager.Instance.targetPooler.GameObjectToPool != null)
        //    TargetsManager.Instance.targetPooler.GameObjectToPool.transform.SetParent(transform);
        //CheckDistanceValue();
    }

    /// <summary>
    /// Проверка значений дистанций до игрока, если минимальное большое макимального, то меняет местами
    /// </summary>
    //private void CheckDistanceValue()
    //{
    //    if (minDistanceToPlayer > maxDistanceToPlayer)
    //    {
    //        var temp = maxDistanceToPlayer;
    //        maxDistanceToPlayer = minDistanceToPlayer;
    //        minDistanceToPlayer = temp;
    //    }        
    //}

    /// <summary>
    /// Переключение с индикатора на игровые объекты
    /// </summary>
    public void SwitchGroundType()
    {
        isGroundIndicatorStatus = !isGroundIndicatorStatus;
        groundIndicatorEmpty.SetActive(isGroundIndicatorStatus);
        groundWithGameObjects.SetActive(!isGroundIndicatorStatus);        
    }

    /// <summary>
    /// Возвращает статус индикатора поверхности
    /// </summary>
    /// <returns>isGroundIndicatorStatus - статус индикатора поверхности</returns>
    public bool GetIndicatorStatus()
    {
        return isGroundIndicatorStatus;
    }

    public void SetKoefScale()
    {
        koefScale = sliderForScaleIndictor.value;

        //if (groundWithGameObjects.activeSelf)
            //groundWithGameObjects.transform.localScale = Vector3.one * koefScale;
            //ARTapToPlaceObject.Instance.transform.localScale = Vector3.one * koefScale;
        transform.localScale = Vector3.one * koefScale;

        ARTapToPlaceObject.Instance.SetKoefScale(koefScale);
    }

    public float GetKoefScale()
    {
        return koefScale;
    }

    public List<GameObject> GetListCoord()
    {
        return listOfDots;
    }
}
