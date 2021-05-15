using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using Wand;
using UnityEngine;

public class TargetsManager : MMSingleton<TargetsManager>, MMEventListener<MMGameEvent>
{
    [SerializeField] public MMSimpleObjectPooler targetPooler;

    Dictionary<GameObject, Target> activeTargets = new Dictionary<GameObject, Target>();
    Dictionary<int, Vector2> arrayCoordinates = new Dictionary<int, Vector2>()   //координатная сетка для вывода целей
    {
        { 1, new Vector2(-1f,-1f) },  { 2, new Vector2(0f,-1f) }, { 3, new Vector2(1f,-1f) }, //first line
        { 4, new Vector2(-1f,0f)  },  { 5, new Vector2(0f,0f)  }, { 6, new Vector2(1f,0f)  }, //second line
        { 7, new Vector2(-1f,1f)  },  { 8, new Vector2(0f,1f)  }, { 9, new Vector2(1f,1f)  }, //third line
    };

    Dictionary<int, Vector2> arrayCellCoordinates16 = new Dictionary<int, Vector2>()   //координатная сетка для вывода целей
    {
        { 1, new Vector2(-1.5f,-1.5f) },  { 2, new Vector2(-0.5f,-1.5f) }, { 3, new Vector2(0.5f,-1.5f) }, { 4, new Vector2(1.5f,-1.5f) }, //first line
        { 5, new Vector2(-1.5f,-0.5f) },  { 6, new Vector2(-0.5f,-0.5f) }, { 7, new Vector2(0.5f,-0.5f) }, { 8, new Vector2(1.5f,-0.5f) }, //second line
        { 9, new Vector2(-1.5f,0.5f)  },  { 10, new Vector2(-0.5f,0.5f) }, { 11, new Vector2(0.5f,0.5f) }, { 12, new Vector2(1.5f,0.5f) }, //third line
        { 13, new Vector2(-1.5f,1.5f) },  { 14, new Vector2(-0.5f,1.5f) }, { 15, new Vector2(0.5f,1.5f) }, { 16, new Vector2(1.5f,1.5f) }, //fourth line

    };


    List<int> activeNumberOfArray = new List<int>(); //список уже активных точек на сетке целей

    private Target pooledTarget;

    private GameObject objectParticle;
    private ParticleSystem particle;

    protected override void Awake()
    {
        base.Awake();
        activeTargets.Clear();
        activeNumberOfArray.Clear();
    }
    
    /// <summary>
    /// Создает цель с указанными параметрами и на указанной позиции.
    /// </summary>
    /// <param name="magicVulnerabilityType">Тип уязвимости цели</param>
    /// <param name="healthAmount">Стартовое количество здоровья</param>
    /// <param name="position">Положение в игровом пространстве.</param>
    public void GenerateTarget(MagicType magicVulnerabilityType, int healthAmount, bool isSpecial)
    {
        if (targetPooler.GetPooledGameObject().TryGetComponent<Target>(out pooledTarget))
        {
            if (activeNumberOfArray.Count <= 9)
            {
                //Debug.Log("1 - " + activeNumberOfArray.Count);
                pooledTarget.SetMaximumHealth(healthAmount);
                Debug.Log("HP - " + healthAmount);
                pooledTarget.SetVulnerability(magicVulnerabilityType);
                pooledTarget.CanChangeVulnerability(isSpecial);

                //test block
                bool canPut = false;
                Vector3 xzDotForTarget = Vector3.zero;
                int numberDot = -1;
                //float koefDots = PlacementIndicatorsDots.Instance.GetKoefScale();
                //List<GameObject> listDots = PlacementIndicatorsDots.Instance.GetListCoord();
                float koefDots = PlacementController.Instance.GetKoefScale();
                List<GameObject> listDots = PlacementController.Instance.GetListCoord();
                
                while (!canPut)
                {
                    numberDot = UnityEngine.Random.Range(1, 16);
                    if (activeNumberOfArray.Count == 0)
                    {
                        //Debug.Log("activeNumberOfArray.Count == 0");
                        numberDot = 6;
                        canPut = true;
                        xzDotForTarget = listDots[numberDot - 1].transform.position;//arrayCellCoordinates16[numberDot] * koefDots;
                        objectParticle = listDots[numberDot - 1];
                        particle = objectParticle.GetComponent<ParticleSystem>(); 
                        //Debug.Log("0 - xzDotForTarget="+ xzDotForTarget);
                    }
                    else
                    {
                        foreach (int item in activeNumberOfArray)
                        {
                            if (item == numberDot)
                            {
                                canPut = false;
                                break;
                            }
                            else
                            {
                                canPut = true;
                                xzDotForTarget = listDots[numberDot - 1].transform.position;//arrayCellCoordinates16[numberDot] * koefDots;
                                objectParticle = listDots[numberDot - 1];
                                particle = objectParticle.GetComponent<ParticleSystem>(); 
                                //Debug.Log("xzDotForTarget=" + xzDotForTarget);
                            }
                        }
                    }
                }
                //Debug.Log("2");
                //end test block

                pooledTarget.SetNumberInArray(numberDot);
                activeNumberOfArray.Add(numberDot);

                Vector3 temp = Vector3.zero;

                //temp.x = position.x + xzDotForTarget.x;
                //temp.y = position.y;
                //temp.z = position.z + xzDotForTarget.y;

                temp = xzDotForTarget;

                pooledTarget.transform.position = temp;// = position.x + xzDotForTarget.x;//new Vector3(xzDotForTarget.x, 0f, xzDotForTarget.y);
                

                //Debug.Log("position = "+ pooledTarget.transform.position);
                pooledTarget.transform.rotation = LookAtPlayer(pooledTarget.transform);

                // test block
                //var scaleValue = 0.5f;
                //if (healthAmount == 1) scaleValue = 0.5f;
                //if (healthAmount == 2) scaleValue = 0.65f;
                //if (healthAmount > 2) scaleValue = 0.8f;
                // end test block
                // test block
                var scaleValue = 0.15f;
                if (healthAmount == 1) scaleValue = 0.15f;
                if (healthAmount == 2) scaleValue = 0.25f;
                if (healthAmount > 2) scaleValue = 0.35f;
                // end test block
                //Debug.Log("3");
                //pooledTarget.transform.localScale = Vector3.one * (scaleValue * (1+ koefDots));

                pooledTarget.transform.localScale = (scaleValue * 10) * PlacementController.Instance.transform.localScale;

                //StartCoroutine(PlayAndStopParticleOneTime());

                particle.Play();

                pooledTarget.gameObject.SetActive(true);

                //particle = objectParticle.GetComponent<ParticleSystem>();

                //StartCoroutine("PlayAndStopParticleOneTime");
                //objectParticle.GetComponent<ParticleSystem>().Play();

                activeTargets.Add(pooledTarget.gameObject, pooledTarget);
            }
        }
    }    

    //private IEnumerator PlayAndStopParticleOneTime()
    //{
    //    Debug.Log("Play Start Particle");
    //    objectParticle.SetActive(true);
    //    particle.Play();
    //    yield return new WaitForSeconds(2f);
    //    particle.Stop();
    //    objectParticle.SetActive(false);
    //    StopCoroutine(PlayAndStopParticleOneTime());
    //}

    public bool CheckIfTargetCanBeDamaged(GameObject target)
    {
        if (activeTargets.ContainsKey(target))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Снимает цель из списка зарегистрированных.
    /// Если списк зарегистрированных целей опустеет в процессе обращения к этому методу, произойдет событие (StaticEvents.allTargetsKilled), сообщающее что все цели уничтоженны.
    /// </summary>
    /// <param name="target">Объект, который мы собираемся снять из списка</param>
    public void SetTargetAsInactive(Target target)
    {
        if (activeTargets.Count == 0) //Если список и так пуст, ничего не делаем.
        {
            return;
        }

        //test block
        if (activeNumberOfArray.Count == 0) return;
        else
        {
            int findEquile = -1;
            foreach (var item in activeNumberOfArray)
            {
                if (item == target.GetNumberInArray())
                {
                    findEquile = item;
                    break;
                }
            }
            if (findEquile != -1)
                activeNumberOfArray.Remove(findEquile);
        }
        //end test block

        activeTargets.Remove(target.gameObject);

        if (activeTargets.Count == 0)
        {
            MMEventManager.TriggerEvent<MMGameEvent>(StaticEvents.allTargetsKilled);
            Debug.Log("All targets killed");
        }
    }

    /// <summary>
    /// Обращаемся к менеджеру целей, чтобы атаковать зарегистрированную цель.
    /// Используй этот метод для ускорения операции обращения к компоненту Health.
    /// </summary>
    /// <param name="damage">Наносимый урон</param>
    /// <param name="magicType">Тип урона</param>
    /// <param name="obj">Объект, который мы атакуем</param>
    /// <returns>Возвращает Истину, если объект получил урон. Возвращает Ложь, если объект не зарегистрирован.</returns>
    public bool DamageRegisteredTarget(int damage, MagicType magicType, GameObject obj)
    {
        if (!CheckIfTargetCanBeDamaged(obj))
            return false;

        activeTargets[obj].DamageTarget(damage, magicType);

        return true;
    }

    private Quaternion LookAtPlayer(Transform spawnedObjectTransform)
    {
        var lookDirection = Camera.main.transform.position - spawnedObjectTransform.position;
        lookDirection.y = 0;
        return Quaternion.LookRotation(lookDirection);
    }

    public void OnMMEvent(MMGameEvent gameEvent)
    {

    }
    private void OnEnable()
    {
        this.MMEventStartListening<MMGameEvent>();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMGameEvent>();
    }
}
