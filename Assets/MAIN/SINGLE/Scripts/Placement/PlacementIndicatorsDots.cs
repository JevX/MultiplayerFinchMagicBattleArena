using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.UI;


public class PlacementIndicatorsDots : MMSingleton<PlacementIndicatorsDots>
{ 
    //Dictionary<int, Vector2> arrayCellCoordinates = new Dictionary<int, Vector2>()   //координатная сетка для вывода целей
    //{
    //    { 1, new Vector2(-1f,-1f) },  { 2, new Vector2(0f,-1f) }, { 3, new Vector2(1f,-1f) }, //first line
    //    { 4, new Vector2(-1f,0f)  },  { 5, new Vector2(0f,0f)  }, { 6, new Vector2(1f,0f)  }, //second line
    //    { 7, new Vector2(-1f,1f)  },  { 8, new Vector2(0f,1f)  }, { 9, new Vector2(1f,1f)  }, //third line
    //};

    Dictionary<int, Vector2> arrayCellCoordinates16 = new Dictionary<int, Vector2>()   //координатная сетка для вывода целей
    {
        { 1, new Vector2(-1.5f,-1.5f) },  { 2, new Vector2(-0.5f,-1.5f) }, { 3, new Vector2(0.5f,-1.5f) }, { 4, new Vector2(1.5f,-1.5f) }, //first line
        { 5, new Vector2(-1.5f,-0.5f) },  { 6, new Vector2(-0.5f,-0.5f) }, { 7, new Vector2(0.5f,-0.5f) }, { 8, new Vector2(1.5f,-0.5f) }, //second line
        { 9, new Vector2(-1.5f,0.5f)  },  { 10, new Vector2(-0.5f,0.5f) }, { 11, new Vector2(0.5f,0.5f) }, { 12, new Vector2(1.5f,0.5f) }, //third line
        { 13, new Vector2(-1.5f,1.5f) },  { 14, new Vector2(-0.5f,1.5f) }, { 15, new Vector2(0.5f,1.5f) }, { 16, new Vector2(1.5f,1.5f) }, //fourth line

    };

    public List<GameObject> listOfDots;
    public float koefScale = 1f;
    public Slider sliderScaleTarget;

    // Start is called before the first frame update
    void Start()
    {
        #region
        //Vector3 xzDotPosition = Vector3.zero;
        //for (int i = 0; i < listOfDots.Count; i++)
        //{
        //    xzDotPosition.x = arrayCellCoordinates16[i+1].x * koefScale;
        //    xzDotPosition.y = 0f;
        //    xzDotPosition.z = arrayCellCoordinates16[i+1].y * koefScale;
        //    listOfDots[i].transform.position = xzDotPosition;
        //}
        #endregion
        SetKoefScale();
    }

    void ChangeTargetScale()
    {
        Vector3 xzDotPosition = Vector3.zero;
        for (int i = 0; i < listOfDots.Count; i++)
        {
            xzDotPosition.x = arrayCellCoordinates16[i + 1].x * koefScale;
            xzDotPosition.y = 0f;
            xzDotPosition.z = arrayCellCoordinates16[i + 1].y * koefScale;
            listOfDots[i].transform.localPosition = xzDotPosition;
            listOfDots[i].transform.localScale = Vector3.one * (koefScale - 0.2f);
            //listOfDots[i].transform.Find("Effect_16_Field ("+(i+1)+")").gameObject.SetActive(false);
        }
    }

    public void SetKoefScale()
    {
        koefScale = sliderScaleTarget.value;
        ChangeTargetScale();
    }

    public void HideHoloTower()
    {
        for (int i = 0; i < listOfDots.Count; i++)
        {
            listOfDots[i].transform.Find("CircleZoneBlue").gameObject.SetActive(false);
        }
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
