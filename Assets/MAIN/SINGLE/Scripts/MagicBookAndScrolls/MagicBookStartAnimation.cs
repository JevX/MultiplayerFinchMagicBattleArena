using UnityEngine;
using DG.Tweening;
using MoreMountains.Tools;
using com.guinealion.animatedBook;
using System.Collections;

public class MagicBookStartAnimation : MMSingleton<MagicBookStartAnimation>, MMEventListener<MMGameEvent>
{
    public GameObject fullMagicBook; //полная книга (левая правая часть)

    public GameObject dotPositionForBook;

    public GameObject tempEndForPosition; // забираем точку на старте анимации

    public GameObject tutorialDrawObject;

    private bool isCanLookAt = false;
    private bool isCanMove = false;

    // Start is called before the first frame update
    void Start()
    {
        DOTween.Init();
        isCanLookAt = false;
        isCanMove = false;

        //tempEndForPosition = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (isCanLookAt)
        {            
            dotPositionForBook.transform.LookAt(Camera.main.transform);
        }

        //if (Vector3.Distance(dotPositionForBook.transform.position, Camera.main.transform.forward) > 3.5f)
        //{
        //    tempEndForPosition.transform.position = Camera.main.transform.forward * 1.1f;// точка куда нужно лететь
        //    isCanMove = true;
        //}

        if (isCanMove)
        {
            InitPosition();            
        }
    }

    public void InitPosition()
    {
        //Vector3 bookShift = Quaternion.Euler(10f, -35f, 0f) * (Camera.main.transform.forward * 1.5f);
        
        Vector3 bookShift = /*Quaternion.Euler(10f, 0f, 0f) */ tempEndForPosition.transform.position;

        dotPositionForBook.transform.position = Vector3.Lerp(dotPositionForBook.transform.position, bookShift, 0.55f * Time.fixedDeltaTime);

        if (Vector3.Distance(dotPositionForBook.transform.position, bookShift) < 0.1f)
        {
            Debug.Log("In Position");
            tutorialDrawObject.SetActive(true);
            isCanMove = false;
        }
    }

    public void ChangeFlagCanMove()
    {
        isCanMove = !isCanMove;
    }

    public void BookStartAnimation()
    {
        Sequence seqAnimBookUp = DOTween.Sequence();

        //fullMagicBook.transform.localScale = Vector3.one * 2.5f;
        //tempEndForPosition.transform.position = Camera.main.transform.forward * 1.1f;

        seqAnimBookUp
            .Append(fullMagicBook.transform.DOMoveY(PlacementController.Instance.transform.localPosition.y+0.5f, 1f)) 
            .OnComplete<Sequence>(StartBookCoroutine);  
    }

    
    public void StartBookCoroutine()
    {        
        StartCoroutine(BookOpenAnimation());
    }

    public IEnumerator BookOpenAnimation()
    {
        //книга открывается
        for (float i = 0; i <= 1; i += 0.05f)
        {
            LightweightBookHelper.Instance.OpenAmmount = i;
            yield return new WaitForSeconds(0.001f);            
        }

        //перелистываются страницы
        for (float i = 0; i < LightweightBookHelper.Instance.PageAmmount; i+=0.05f)
        {
            LightweightBookHelper.Instance.Progress = i;
            yield return new WaitForSeconds(0.001f);
        }

        LightweightBookHelper.Instance.Progress = 0f;

        isCanLookAt = true; //можно следить за игроком не сходя с места (поворачивается в сторону игрока)
        isCanMove = true;   //можно перемещаться
       
        dotPositionForBook.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        dotPositionForBook.transform.position = fullMagicBook.transform.position;

        
        fullMagicBook.transform.SetParent(dotPositionForBook.transform);
        fullMagicBook.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        fullMagicBook.transform.localPosition = Vector3.zero;

        dotPositionForBook.transform.localScale = Vector3.one * 2f;

        //TutorialSteps.Instance.Step6_Done();

        //TutorialStep6_InfoTarget.Instance.HideStep(); //если что вернуть
    }    

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        //if (gameEvent.EventName == "GameStarted")
        //{
        //    BookStartAnimation();
        //}
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
