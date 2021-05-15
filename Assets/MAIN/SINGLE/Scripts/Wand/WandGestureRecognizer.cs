using DigitalRubyShared;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wand;
using UnityEngine;
using UnityEngine.UI;

public class WandGestureRecognizer : ImageGestureRecognizerComponentScript
{
    private ImageGestureImage matchedImage;
    public Text magicText;
    public MagicWand myWand;

    /// <summary>
    /// This dictionary contains the list of touches that we are tracking, using id to gesture lookup.
    /// For the mouse we are only tracking the left pointer and using id -999 to track it, since
    /// Unity input touches have positive id values.
    /// </summary>
    private readonly Dictionary<int, GestureTouch> virtualTouches = new Dictionary<int, GestureTouch>();
    private const int virtualGestureID = -999;
    private readonly List<int> idsToRemove = new List<int>();

    Vector2[] virtualGesturePositions;
    Vector2 currentVirtualPosition;
    UnityEngine.TouchPhase currentVirtualPhase;

    bool virtualInputInProgress;

    bool readyToBeUpdated = false;

    Vector3 GlyphSpawnPoint = Vector3.zero;
    public MMMiniObjectPooler glyphPool;

    private GameObject projectionPlaneHelper;
    float glyphSizeModifier;

    public float glyphSizeShift = 0.5f;
    public float glyphDistanceShift = 0.5f;

    bool magicIsCasting;

    Vector3[] glyphImage = new Vector3[0];

    public bool RecognizeSimplifiedImage;
    public bool DrawSimplifiedImage;

    [FMODUnity.EventRef]
    public string FMODCasting;

    public string GestureForIce;
    public string GestureForFire;
    public string GestureForElectro;
    public string GestureForKinetic;

    public ParticleSystem ElectroExplosion;
    public ParticleSystem FireExplosion;
    public ParticleSystem IceExplosion;
    public ParticleSystem KineticExplosion;

    public ParticleSystem EffectActivated;
    public ParticleSystem.MainModule mainModule;

    [FMODUnity.EventRef]
    public string FMODFireEnchantment;
    [FMODUnity.EventRef]
    public string FMODIceEnchantment;
    [FMODUnity.EventRef]
    public string FMODElectroEnchantment;
    [FMODUnity.EventRef]
    public string FMODKineticEnchantment;

    private FMOD.Studio.EventInstance FMODInstance;

    //private FMOD.Studio.EventInstance FMODInstance;

    protected override void Awake()
    {
        base.Awake();
        projectionPlaneHelper = new GameObject();
        projectionPlaneHelper.name = "ProjectionPlaneHelper";
        projectionPlaneHelper.transform.position = Vector3.zero;
    }

    public void RegisterMe(MagicWand magicWand)
    {
        myWand = magicWand;
    }

    /// <summary>
    /// Получает данные о гестуре, которую мы хотим эмулировать. Сразу после получения запускает процесс распознавания.
    /// </summary>
    /// <param name="simplifiedDrawing"></param>
    public void InsertGestureData(Vector3[] simplifiedDrawing, Vector3[] originalDrawing)
    {
        if (magicIsCasting)
            return;

        //Делаем проекции по направлению взгляда игрока. Это может быть не обязательно камера, но и любой другой объект/вектор
        Vector2[] projectedSimplifiedImage = ProjectWorldDrawingOntoPlane(Camera.main.transform.forward, simplifiedDrawing, true, out glyphSizeModifier);
        Vector2[] projectedOriginalImage = ProjectWorldDrawingOntoPlane(Camera.main.transform.forward, originalDrawing, true, out glyphSizeModifier);

        glyphImage = new Vector3[DrawSimplifiedImage ? projectedSimplifiedImage.Length : projectedOriginalImage.Length];
        for (int i = 0; i < glyphImage.Length; i++)
        {
            glyphImage[i] = DrawSimplifiedImage ? projectedSimplifiedImage[i] : projectedOriginalImage[i];
        }

        virtualGesturePositions = RecognizeSimplifiedImage ? projectedSimplifiedImage : projectedOriginalImage;
        virtualGesturePositions = ScaleDataToScreenSpace(virtualGesturePositions);

        GlyphSpawnPoint = CalculatePointsCenter(originalDrawing);

        FMODUnity.RuntimeManager.PlayOneShot(FMODCasting, GlyphSpawnPoint);

        List<float> xy = new List<float>();
        for (int i = 0; i < virtualGesturePositions.Length; i ++)
        {
            xy.Add(virtualGesturePositions[i].x);
            xy.Add(virtualGesturePositions[i].y);
        }
        Gesture.Simulate(xy.ToArray());
    }

    private Vector3 CalculatePointsCenter(Vector3[] points)
    {
        Vector3 centerPosition = Vector3.zero;
        for (int i = 0; i < points.Length; i++)
        {
            centerPosition += points[i];
        }
        return centerPosition / points.Length;
    }

    private Vector3 CalculatePointsCenter(Vector2[] points)
    {
        return CalculatePointsCenter(points);
    }

    private Vector2[] ScaleDataToScreenSpace(Vector2[] pointsData)
    {
        for (int i = 0; i < pointsData.Length; i++)
        {
            pointsData[i].x *= Screen.width;
            pointsData[i].y *= Screen.height;
        }

        return pointsData;
    }

    /// <summary>
    /// Проекция массива точек на указанную плоскость по вектору.
    /// </summary>
    /// <param name="planeVector">Вектор плоскости (нормаль)</param>
    /// <param name="originalPoints">Массив исходных точек</param>
    /// <param name="projectedData">Массив, в который требуется записать результат</param>
    private Vector2[] ProjectWorldDrawingOntoPlane(Vector3 planeVector, Vector3[] originalPoints, bool normalize, out float sizeModifier)
    {
        projectionPlaneHelper.transform.forward = planeVector;

        Vector2[] projectedData = new Vector2[originalPoints.Length];
        Vector3[] modifiedPoints = new Vector3[originalPoints.Length];

        Vector2 boundTopRight = Vector2.zero;
        Vector2 boundDownleft = Vector2.zero;

        for (int i = 0; i < modifiedPoints.Length; i++)
        {
            modifiedPoints[i] = projectionPlaneHelper.transform.InverseTransformPoint(originalPoints[i]);
            modifiedPoints[i].z = 0;

            if (i == 0)
            {
                boundTopRight = modifiedPoints[0];
                boundDownleft = modifiedPoints[0];
            }

            if (modifiedPoints[i].x > boundTopRight.x)
                boundTopRight.x = modifiedPoints[i].x;
            if (modifiedPoints[i].y > boundTopRight.y)
                boundTopRight.y = modifiedPoints[i].y;

            if (modifiedPoints[i].x < boundDownleft.x)
                boundDownleft.x = modifiedPoints[i].x;
            if (modifiedPoints[i].y < boundDownleft.y)
                boundDownleft.y = modifiedPoints[i].y;
        }

        float xSize = boundTopRight.x - boundDownleft.x;
        float ySize = boundTopRight.y - boundDownleft.y;

        sizeModifier = (xSize > ySize ? xSize : ySize);

        for (int i = 0; i < originalPoints.Length; i++)
        {
            modifiedPoints[i].x -= boundDownleft.x;
            modifiedPoints[i].y -= boundDownleft.y;

            if (normalize)
            {
                float vectorModifier = 1 / sizeModifier;

                projectedData[i].x = modifiedPoints[i].x * vectorModifier;
                projectedData[i].y = modifiedPoints[i].y * vectorModifier;
            }
        }

        Debug.Log("Projection complete");
        return projectedData;
    }

    private void CheckMatсhedGesture()
    {
        if (matchedImage == null)
        {
            Debug.Log("No match found!");
        }
        else
        {
            MagicType selectedType = MagicType.None;
            mainModule = EffectActivated.main;

            if (matchedImage.Name == GestureForIce)
            {
                selectedType = MagicType.Ice;
                Instantiate(IceExplosion).transform.position = EffectActivated.transform.position;
                FMODUnity.RuntimeManager.PlayOneShot(FMODIceEnchantment, transform.position);

                mainModule.startColor = MagicColors.ice;
            }
            else if(matchedImage.Name == GestureForFire)
            {
                selectedType = MagicType.Fire;
                Instantiate(FireExplosion).transform.position = EffectActivated.transform.position;
                FMODUnity.RuntimeManager.PlayOneShot(FMODFireEnchantment, transform.position);

                mainModule.startColor = MagicColors.fire;
            }
            else if(matchedImage.Name == GestureForKinetic)
            {
                selectedType = MagicType.Kinetic;
                Instantiate(KineticExplosion).transform.position = EffectActivated.transform.position;
                FMODUnity.RuntimeManager.PlayOneShot(FMODKineticEnchantment, transform.position);

                mainModule.startColor = MagicColors.kinetic;
            }
            else if(matchedImage.Name == GestureForElectro)
            {
                selectedType = MagicType.Electro;
                Instantiate(ElectroExplosion).transform.position = EffectActivated.transform.position;
                FMODUnity.RuntimeManager.PlayOneShot(FMODElectroEnchantment, transform.position);

                mainModule.startColor = MagicColors.electro;
            }

            if (selectedType == MagicType.None)
            {
                Reset();
                return;
            }

            EffectActivated.Play();

            myWand.ChangeMagicType(selectedType);

            // image gesture must be manually reset when a shape is recognized
            Reset();
        }
    }

    private void SpawnGlyph(MagicType selectedType)
    {
        if (glyphImage.Length > 1)
        {
            GameObject pooledGlyph = glyphPool.GetPooledGameObject();
            if (pooledGlyph != null)
            {
                pooledGlyph.transform.localScale = Vector3.one * glyphSizeModifier * glyphSizeShift;
                pooledGlyph.transform.position = GlyphSpawnPoint;
                pooledGlyph.transform.LookAt(Camera.main.transform);
                //pooledGlyph.transform.localRotation *= Quaternion.Euler(0, 180, 0);
                pooledGlyph.transform.position += -pooledGlyph.transform.forward * glyphDistanceShift;
                pooledGlyph.SetActive(true);

                //Debug.Log("You drew a " + matchedImage.Name);

                pooledGlyph.GetComponent<EnchantmentGlyph>().SetGlyph(selectedType, glyphImage);
                glyphImage = new Vector3[0];
            }
        }
    }

    /// <summary>
    /// Callback for gesture events
    /// </summary>
    /// <param name="gesture">Gesture</param>
    public void GestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            // save off the matched image, the gesture may reset if max path count has been reached
            virtualInputInProgress = false;
            matchedImage = Gesture.MatchedGestureImage;
            CheckMatсhedGesture();
        }
        else if (gesture.State != GestureRecognizerState.Began && gesture.State != GestureRecognizerState.Executing)
        {
            // don't update lines unless executing
            return;
        }
    }

    /// <summary>
    /// Reset state, clear all lines
    /// </summary>
    public void Reset()
    {
        Gesture.Reset();
    }

    private void AddTouch(int id, Vector2 position, UnityEngine.TouchPhase phase, float pressure = 1.0f)
    {
        GestureTouch touch = FingersScript.Instance.GestureTouchFromVirtualTouch(id, position, phase, pressure);
        virtualTouches[id] = touch;
    }

    private void ProcessVirtualGesture()
    {
        if (virtualInputInProgress)
        {
            AddTouch(virtualGestureID, currentVirtualPosition, currentVirtualPhase);
            readyToBeUpdated = true;
        }
    }

    private void RemoveExpiredTouches()
    {
        // It is critical that you remove any of your touches that have entered the Ended phase
        // This should be done in the VirtualTouchCountHandler callback on FingersScript.Instance.
        foreach (GestureTouch touch in virtualTouches.Values)
        {
            if (touch.TouchPhase == DigitalRubyShared.TouchPhase.Ended || touch.TouchPhase == DigitalRubyShared.TouchPhase.Cancelled || touch.TouchPhase == DigitalRubyShared.TouchPhase.Unknown)
            {
                idsToRemove.Add(touch.Id);
            }
        }
        foreach (int id in idsToRemove)
        {
            virtualTouches.Remove(id);
        }
        idsToRemove.Clear();
    }

    private int GetVirtualTouchCount()
    {
        ProcessVirtualGesture();

        return virtualTouches.Count;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Gesture.StateUpdated -= GestureCallback;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Gesture.StateUpdated += GestureCallback;

        // setup the virtual touch handlers, this is required to link fingers script to your virtual touches
        FingersScript.Instance.VirtualTouchCountHandler = GetVirtualTouchCount;
        FingersScript.Instance.VirtualTouchObjectHandler = (index) => virtualTouches.ElementAt(index).Value;
        FingersScript.Instance.VirtualTouchUpdateHandler = () => RemoveExpiredTouches();
        FingersScript.Instance.VirtualTouchResetHandler = () => virtualTouches.Clear();
    }
}
