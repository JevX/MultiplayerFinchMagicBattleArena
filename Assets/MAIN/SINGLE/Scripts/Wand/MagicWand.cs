using DigitalRubyShared;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Finch;

public class MagicWand : MMSingleton<MagicWand>
{
    public int Damage = 1;
    public MagicType magicType;
    [SerializeField] private LineRenderer DrawingLineRenderer;

    [SerializeField] public Transform wandEndPosition;

    public int limitDrawingPoints = 60;
    public float simplifyTolerance = 1f;

    public float drawingSmoothness = 0.2f;
    public bool isDrawing { get; private set; }

    MagicShooter_Manager magicShooter;
    WandInput wandInput;
    public WandGestureRecognizer wandGesture;

    public GameObject FireEffect;
    public GameObject IceEffect;
    public GameObject ElectroEffect;
    public GameObject KineticEffect;

    private List<Vector3> pointsList = new List<Vector3>();


    public ParticleSystem DrawVFX;

    public Material KineticMaterial;

    bool GlowActivated;

    public Color blackColor;
    public Color glowColor;

    [SerializeField] private LaserSight laserSight;

    protected override void Awake()
    {
        base.Awake();

        wandGesture = GetComponent<WandGestureRecognizer>();
        magicShooter = GetComponent<MagicShooter_Manager>();
        wandInput = GetComponent<WandInput>();
        wandInput.RegisterMe(this);
        wandGesture.RegisterMe(this);

        ChangeMagicType(MagicType.None);
    }

    /// <summary>
    /// Сброс нарисованного игроком глифа. Очищает игровое пространство.
    /// </summary>
    public void ClearAndStopDrawing()
    {
        Debug.Log("Drawing cleared");
        DrawingLineRenderer.positionCount = 0;
        pointsList.Clear();
        isDrawing = false;
        DrawVFX.Stop();
    }

    /// <summary>
    /// Изменяет тип магии палочки на указанный
    /// </summary>
    /// <param name="type">Тип магии</param>
    public void ChangeMagicType(MagicType type)
    {
        if (magicType == type) return;


        FireEffect.SetActive(false);
        IceEffect.SetActive(false);
        ElectroEffect.SetActive(false);
        KineticEffect.SetActive(false);
        HideKineticObjects();

        magicShooter.ResetAllShooters();

        magicType = type;

        switch (magicType)
        {
            case MagicType.None:
                laserSight.ChangeLaserColor(Color.white);
                break;

            case MagicType.Electro:
                ElectroEffect.SetActive(true);
                laserSight.ChangeLaserColor(MagicColors.electro);
                break;

            case MagicType.Fire:
                FireEffect.SetActive(true);
                laserSight.ChangeLaserColor(MagicColors.fire);
                break;

            case MagicType.Ice:
                IceEffect.SetActive(true);
                laserSight.ChangeLaserColor(MagicColors.ice);
                break;

            case MagicType.Kinetic:
                KineticEffect.SetActive(true);
                laserSight.ChangeLaserColor(MagicColors.kinetic);
                ShowKineticObjects();
                break;
        }

        MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.magicChanged);
    }

    /// <summary>
    /// Начинает магическое заклинание. Пока доступно только для Fireball.
    /// </summary>
    public void BeginMagic()
    {
        magicShooter.BeginCast(magicType);
    }

    /// <summary>
    /// Заканчивает магическое заклинание.
    /// </summary>
    public void CastDone()
    {
        magicShooter.CastDone(magicType);
    }

    /// <summary>
    /// Запускает процесс рисования.
    /// </summary>
    public void StartDraw()
    {
        ClearAndStopDrawing();
        isDrawing = true;
        DrawVFX.Play();
    }

    /// <summary>
    /// Завершает процесс рисования. Завершая процесс рисования, мы преобразуем нарисованный игроком объект в серию векторов для эмуляции гестуры.
    /// </summary>
    public void EndDraw()
    {
        if (DrawingLineRenderer.positionCount < 3)
        {
            ClearAndStopDrawing();
            return;
        }
        //Собираем точки из рисунка человека.


        Vector3[] originalImage = new Vector3[DrawingLineRenderer.positionCount];
        DrawingLineRenderer.GetPositions(originalImage);


        DrawingLineRenderer.Simplify(simplifyTolerance);
        Debug.Log("Positions simplified: " + DrawingLineRenderer.positionCount);

        Vector3[] simplifiedImage = new Vector3[DrawingLineRenderer.positionCount];
        DrawingLineRenderer.GetPositions(simplifiedImage);

        wandGesture.InsertGestureData(simplifiedImage, originalImage);

        ClearAndStopDrawing();
    }

    private void FixedUpdate()
    {
        //Процесс рисования
        if (isDrawing)
        {
            laserSight.SetLaserOff();

            //Если позиция нулевая, начинаем рисовать
            if (pointsList.Count == 0)
            {
                pointsList.Add(wandEndPosition.position);
                UpdatePoints();
            }
            else if (Vector3.Distance(pointsList[pointsList.Count - 1], wandEndPosition.position) > drawingSmoothness)
            {
                pointsList.Add(wandEndPosition.position);
                if (pointsList.Count > limitDrawingPoints)
                {
                    pointsList.RemoveAt(0);
                }
                UpdatePoints();
                //Рисуем в зависимости от удаленности поинтов
            }
        }
        else
        {
            laserSight.SetLaserON();
        }
    }

    private void UpdatePoints()
    {
        DrawingLineRenderer.positionCount = pointsList.Count;
        DrawingLineRenderer.SetPositions(pointsList.ToArray());
    }


    public void ShowKineticObjects()
    {
        GlowActivated = true;
        StartCoroutine(GlowActive());
    }

    public void HideKineticObjects()
    {
        GlowActivated = false;
        KineticMaterial.SetColor("_Color1", blackColor);
    }

    IEnumerator GlowActive()
    {
        float pos = 0;
        bool direction = true;
        while (GlowActivated)
        {
            KineticMaterial.mainTextureOffset += Vector2.right * Time.deltaTime;
            KineticMaterial.SetColor("_Color1", Color.Lerp(blackColor, glowColor, pos));
            pos += direction ? Time.deltaTime : -Time.deltaTime;
            if (pos > 1)
            {
                direction = false;
            }
            if (pos < 0)
            {
                direction = true;
            }
            yield return null;
        }
    }
}
