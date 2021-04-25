using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class help3dofLineMarker : MonoBehaviour 
{
    public static help3dofLineMarker Instance;

    [Header("Lines")]
    public LineRenderer LineHelp;
    
    
    [Header("Color gradient settings")]
    public float AlphaStart = 0.85f;
    public float AlphaEnd   = 0.5f;
    public Color ColorStart = Color.white;
    public Color ColorEnd   = Color.blue;
    
    [Header("Activation settings")]
    public float ActivationSpeed = 10;
    
    [Header("Public properties")]
    public bool bAbleToSelectNew = true;
    

    /////////////////////////////////////////////////////////////////
    // Private
    /////////////////////////////////////////////////////////////////

    private Finch.FinchCalibration calibration;
    private LineRenderer LineController;
    // Origin Settings (Pyramid)
    private GameObject OriginAnchor_Center; 
    private GameObject OriginAnchor_Top_RadiusDefine; 


    // GC preserve 
    private Gradient            gradient  = new Gradient();
    private GradientColorKey[]  colorKeys = new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f),    new GradientColorKey(Color.white, 1.0f)    };
    private GradientAlphaKey[]  alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f),           new GradientAlphaKey(1.0f, 1.0f)           };

    private float m_activation = 0;

    void Awake()
    {
        Instance = this;
    }
   
    // Use this for initialization
    void Start () 
    {
        // Calibration
        calibration                     = GameObject.Find("FinchCalibration")                  .GetComponent<Finch.FinchCalibration>();
         // Controller Line 
        LineController                  = GameObject.Find("Controller")                        .GetComponent<LineRenderer>();
        // Podium 
        OriginAnchor_Center             = GameObject.Find("PodiumOriginAnchor_Center");
        OriginAnchor_Top_RadiusDefine   = GameObject.Find("PodiumOriginAnchor_Top_RadiusDefine");
    }


    // Update is called once per frame
    void Update () 
    {
        if (OriginAnchor_Center == null)
        {
            OriginAnchor_Center = GameObject.Find("PodiumOriginAnchor_Center");
            OriginAnchor_Top_RadiusDefine = GameObject.Find("PodiumOriginAnchor_Top_RadiusDefine");
            return;
        }
        bool b3dof = (Finch.FinchNodeManager.GetUpperArmCount() == 0);
        
        // support only 3df
        if(!b3dof)
        {
            bAbleToSelectNew = false;
            LineHelp.enabled = false;
        }
        else
        {
            LineHelp.enabled = true;

            // start
            Vector3 V0 = LineController.transform.TransformPoint(LineController.GetPosition(0));
            
            // end
            Vector3 V1 = LineController.transform.TransformPoint(LineController.GetPosition(1));

            Vector3 origin = OriginAnchor_Center.transform.position;
            Vector3 origin_top = OriginAnchor_Top_RadiusDefine.transform.position;
            float radius = (origin_top - origin).magnitude;

            // project on line
            Vector3 projected = ProjectPointLine(origin, V0, V1);

            // little move origin towards projected - little visual pleasing
            Vector3 start_line_pos = origin + (projected-origin).normalized * radius;



            // Update help line position
            {
                LineHelp.SetPosition(0, LineHelp.transform.InverseTransformPoint(start_line_pos));
                LineHelp.SetPosition(1, LineHelp.transform.InverseTransformPoint(projected));
            }


            // Update line gradient
            {
                colorKeys[0].color = ColorStart;
                colorKeys[1].color = ColorEnd;

                alphaKeys[0].alpha = AlphaStart * m_activation;
                alphaKeys[1].alpha = AlphaEnd * m_activation;

                gradient.SetKeys(colorKeys, alphaKeys);
                LineHelp.colorGradient = gradient;
            }
        }

        // Interpolate activation 
        {
            m_activation = Lerp(m_activation, bAbleToSelectNew ? 1 : 0, Time.deltaTime, ActivationSpeed); 
        }
    }

    

    /////////////////////////////////////////////////////////////////
    // @todo: MOVE IT TO YOUR MATH LIBRARY
    /////////////////////////////////////////////////////////////////

    float Lerp(float Current, float Target, float DeltaTime, float InterpSpeed)
    {
        if (InterpSpeed <= 0.0f) return Target;
        float Alpha = Mathf.Clamp(DeltaTime * InterpSpeed, 0.0f, 1.0f);
        return Target * Alpha + Current * (1.0f - Alpha);
    }

    float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        return Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
    }

    Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 rhs = point - lineStart;
        Vector3 vector2 = lineEnd - lineStart;
        float magnitude = vector2.magnitude;
        Vector3 lhs = vector2;
        if (magnitude > 1E-06f)
        {
            lhs = (Vector3)(lhs / magnitude);
        }
        float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
        return (lineStart + ((Vector3)(lhs * num2)));
    }
}