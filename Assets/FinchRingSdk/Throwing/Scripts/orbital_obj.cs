using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbital_obj : MonoBehaviour
{
    Transform OrbitalCenter;
    public int CountOrbitalFigure = 5;

    public float HeightMin = -1;
    public float HeightMax = 1;

    public float Speed = 30;
    public float speedModForCenter = 3;
   // public Vector3 rotateFigure;
   // public float up = 1;
    public float MinRadius = 2;
    public float MaxRadius = 9;

    [Header("Scale orbital figure")]
    public float minScale;
    public float maxScale;
    // public static Vector3 centerForOrbital;
   // public float ScaleFigure = 0.5f;
    public List<GameObject> OrbitalFiguresList = new List<GameObject>();
    List<GameObject> figList = new List<GameObject>();
    
    
    void Start()
    {
        OrbitalCenter = gameObject.transform;
        for (int i = 0; i < CountOrbitalFigure; i++)
        {
            GameObject figure = new GameObject();
            figure = Instantiate(OrbitalFiguresList[Random.Range(0, OrbitalFiguresList.Count)]);

            float Scalefactor = Random.Range(minScale, maxScale);
            figure.transform.localScale = new Vector3(Scalefactor, Scalefactor, Scalefactor);
            figure.transform.parent = gameObject.transform;
            figure.transform.GetChild(0).gameObject.AddComponent<rotatioOrbitalObj>();


            //int i = figList.Count;
            float k = Random.value;
            //float dt = 1.0f / (float)figList.Count;
            //k = i * dt + k*dt; 


            float angle_x = k * 2 * Mathf.PI;
            float dist_v = Mathf.Lerp(MinRadius, MaxRadius, Random.value);
            float px = Mathf.Cos(angle_x);
            float py = Mathf.Sin(angle_x);
            float height_y = Mathf.Lerp(HeightMin, HeightMax, Random.value);

            Vector3 relative = new Vector3(px * dist_v, height_y, py * dist_v);
            figure.transform.position = relative + OrbitalCenter.position;


            figList.Add(figure);
        }
    }

    void Update()
    {
        OrbitalCenter = gameObject.transform;
        //// centerForOrbital = transform.position;
        //if (figList.Count < CountOrbitalFigure)
        //{
        //    GameObject figure = new GameObject();
        //    figure = Instantiate(OrbitalFiguresList[Random.Range(0,OrbitalFiguresList.Count)]);
            
        //    float Scalefactor = Random.Range(minScale, maxScale);
        //    figure.transform.localScale = new Vector3(Scalefactor,Scalefactor,Scalefactor);
            
        //    figure.transform.GetChild(0).gameObject.AddComponent<rotatioOrbitalObj>();


        //    //int i = figList.Count;
        //    float k = Random.value;
        //    //float dt = 1.0f / (float)figList.Count;
        //    //k = i * dt + k*dt; 


        //    float angle_x = k * 2 * Mathf.PI;
        //    float dist_v = Mathf.Lerp(MinRadius, MaxRadius, Random.value);
        //    float px = Mathf.Cos(angle_x);
        //    float py = Mathf.Sin(angle_x);
        //    float height_y = Mathf.Lerp(HeightMin, HeightMax, Random.value);
            
        //    Vector3 relative = new Vector3(px * dist_v, height_y, py * dist_v);
        //    figure.transform.position = relative + OrbitalCenter.transform.position;


        //    figList.Add(figure);
            
            
        //}

        //if (figList.Count > CountOrbitalFigure)
        //{
        //    if (CountOrbitalFigure >= 0)
        //    {
        //        Destroy(figList[0]);
        //        figList.RemoveAt(0);
        //    }
        //}

        foreach (GameObject i in figList)
        {
            i.transform.LookAt(OrbitalCenter.transform.position);
            i.transform.Translate(Vector3.right * Speed * Time.deltaTime);
           // i.transform.localScale = new Vector3(ScaleFigure, ScaleFigure, ScaleFigure);
            //i.transform.GetChild(0).transform.Rotate(rotateFigure.x * Time.deltaTime,rotateFigure.y * Time.deltaTime, rotateFigure.z * Time.deltaTime);

            Vector3 directionToCenter = OrbitalCenter.transform.position - i.transform.position;
            if (directionToCenter.magnitude > MaxRadius)
            {
                // print("min");
                i.transform.Translate(Vector3.forward * speedModForCenter * Time.deltaTime, Space.Self);
            }

            if (directionToCenter.magnitude < MinRadius)
            {
                // print("max");
                i.transform.Translate(-Vector3.forward * speedModForCenter * Time.deltaTime, Space.Self);
            }
        }
    }
}
