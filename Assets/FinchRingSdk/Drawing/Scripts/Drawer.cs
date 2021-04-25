using UnityEngine;
using Finch;
using UnityEngine.Audio;

public static class FMath
{
    public static float FInterpTo(float Current, float Target, float DeltaTime, float InterpSpeed)
    {
        if (InterpSpeed <= 0f)
        {
            return Target;
        }
        float Alpha = Mathf.Clamp01(DeltaTime * InterpSpeed);
        return Mathf.Lerp(Current, Target, Alpha);
    }
}

public class Drawer : MonoBehaviour
{
    /// <summary>
    /// Left Drawer instance
    /// </summary>
    public static Drawer LeftDrawer;

    /// <summary>
    /// Right Drawer instance
    /// </summary>
    public static Drawer RightDrawer;

    [Header("Main settings")]
    /// <summary>
    /// Current Drawer chirality
    /// </summary>
    public Chirality Chirality = Chirality.Right;

    [Header("Pen Settings")]

    /// <summary>
    /// Pen mesh renderer
    /// </summary>
    public MeshRenderer Pen;

    /// <summary>
    /// Virtual pen mesh renderer
    /// </summary>
    public MeshRenderer PenVirtual;


    /// <summary>
    /// Draw point Transform
    /// </summary>
    public Transform DrawPoint;

    /// <summary>
    /// Virtual draw point Transform
    /// </summary>
    public Transform DrawPointVirtual;

    /// <summary>
    /// Scale of Pen mesh
    /// </summary>
    public float PenScale = 1.35f;

    // Visual elements of Drawer
    public UPenParticles[] PenPaintParticles =  new UPenParticles[3];

    public Material[] StylusMats_Body = new Material[3];
    public Material[] StylusMats_Glow = new Material[3];
    public Material[] LineMaterial = new Material[3];

    public AudioSource[] PenSparksAudio;

    [HideInInspector]
    public bool DrawingState;

    [HideInInspector]
    public bool RecalibrationEnd;

    [HideInInspector]
    public bool HoverOnWall;

    /// <summary>
    /// Line Collision instance
    /// </summary>
    public LineCollision LCollision;
    
    /// <summary>
    /// Current collor instance
    /// </summary>
    public int ColorKey = 2; // red

    private DrawLine drawLine;

    private bool lastPointOnWall;
    private bool startDrawing;

    private float StylusActiveCurrent = 0;
    private float[] StylusSparksCurrent = new float[3] { 0,0,0};
    private float DrawingOnWallHighlight = 0;

    private static float offsetZ = 0.0003f;
    private const float radius = 0.01f;

    private void Awake()
    {
        if (Chirality == Chirality.Left)
        {
            LeftDrawer = this;
        }

        if (Chirality == Chirality.Right)
        {
            RightDrawer = this;
        }
    }

    //TODO: Change color of drawed line and stylus visualisation in runtime

    private void LateUpdate()
    {
        //Checking current states of buttons
        bool controllerEnable = LCollision.Enabled;
        bool pressed = FinchController.GetPress(Chirality, RingElement.HomeButton);
        bool wasPressed = FinchController.GetPressDown(Chirality, RingElement.HomeButton) || FinchController.GetPressTime(Chirality, RingElement.HomeButton) > .1f;

        if (!RecalibrationEnd && pressed)
            return;
        else
            RecalibrationEnd = true;

        startDrawing |= controllerEnable && wasPressed;
        startDrawing &= pressed;

        if (FinchController.GetPressUp(Chirality, RingElement.HomeButton))
            startDrawing = false;

        //Activates and update visual elements
        Pen.gameObject.SetActive(DrawingManager.IsDrawing && controllerEnable);
        PenVirtual.gameObject.SetActive(false);

        DrawingOnWallHighlight = FMath.FInterpTo(DrawingOnWallHighlight, DrawingState || HoverOnWall ? 1 : 0, Time.deltaTime, 3f);

        for (int i = 0; i < 3; ++i)
            StylusSparksCurrent[i] = FMath.FInterpTo(StylusSparksCurrent[i], pressed && (ColorKey == i) ? 1 : 0, Time.deltaTime, 6f);

        StylusActiveCurrent = FMath.FInterpTo(StylusActiveCurrent, pressed ? 1 : 0, Time.deltaTime, 8f);
        var mats = Pen.sharedMaterials;
        mats[1] = StylusMats_Body[ColorKey];
        mats[0] = StylusMats_Glow[ColorKey];

        mats[1].SetFloat(FShaderIDs.g_fStylusActive, StylusActiveCurrent);
        mats[0].SetFloat(FShaderIDs.g_fActive, StylusActiveCurrent);
        Pen.sharedMaterials = mats;

        for (int i = 0; i < PenPaintParticles.Length; ++i)
        {
            PenPaintParticles[i].Emit = ColorKey == i && pressed;
            PenPaintParticles[i].transform.position = DrawPoint.transform.position;
        }

        for (int i = 0; i < PenSparksAudio.Length; ++i)
        {
            PenSparksAudio[i].volume = StylusSparksCurrent[i];
        }

        DrawPoint.transform.position = LCollision.PointPosition;

        //Updating Transform of Pen and DrawPoint
        Quaternion penRotation = DrawPoint.transform.rotation * Quaternion.Euler(0, -180, 0);
        Pen.transform.position = DrawPoint.transform.position;
        Pen.transform.rotation = penRotation;
        Pen.transform.localScale = Vector3.one * PenScale;

        //Draw and update the line
        UpdateLine(controllerEnable && startDrawing);

        if (!controllerEnable || !startDrawing)
        {
            startDrawing = false;
            drawLine = null;
            offsetZ += 0.000003f;
        }
    }

    /// <summary>
    /// Update drawing line
    /// </summary>
    /// <param name="draw"></param>
    private void UpdateLine(bool draw)
    {
        Vector3 basePosition;

        if (LCollision.PointPosition != null && FinchNodeManager.GetUpperArmCount() == 0)
            basePosition = LCollision.PointPosition;
        else
            basePosition = DrawPointVirtual.transform.position;

        Quaternion baseRotation = DrawPoint.transform.rotation;

        if (DrawingState || HoverOnWall)
        {
            bool pointOnWall = false;
            lastPointOnWall |= pointOnWall;
            Vector3 position = new Vector3();
            UpdateLinesState(position, baseRotation, true);

            lastPointOnWall = false;

            if (draw)
            {
                drawLine = null;
                offsetZ += 0.000003f;
            }

        }
        else if (draw)
        {
            UpdateLinesState(basePosition, baseRotation, false);
        }
    }

    /// <summary>
    /// Update state of drawed lines
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="ignoreDistance"></param>
    private void UpdateLinesState(Vector3 position, Quaternion rotation, bool ignoreDistance)
    {
        if (DrawingManager.IsDrawing)
        {
            if (drawLine == null)
            {
                var lineMaterial = LineMaterial[ColorKey];
                drawLine = DrawLine.CreateLine(position, rotation, DrawingManager.DrawingBase, DrawingState, lineMaterial);
            }

            Vector2 palelte01Coords = Vector2.zero;

            Vector3 direction = rotation * Vector3.forward;

            drawLine.UpdateLineDrawing(position, direction, radius, palelte01Coords, ignoreDistance);
        }
    }
}
