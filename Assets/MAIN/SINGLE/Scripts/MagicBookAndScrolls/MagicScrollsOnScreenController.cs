using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Finch;
using com.guinealion.animatedBook;
using MoreMountains.Tools;

public class MagicScrollsOnScreenController : MMSingleton<MagicScrollsOnScreenController>
{
    //public List<GameObject> listOfMagicScrolls;
    //public Image imageScroll;
    private float currentCount = 0f;
    private int prevCount = 0;
    private int nextCount = 0;

    NodeType Node;

    private void Start()
    {
        //currentCount = 0;
        //nextCount = 1;
        //prevCount = listOfMagicScrolls.Count-1;

        //for (var i = 0; i < listOfMagicScrolls.Count; i++)
        //    listOfMagicScrolls[i].SetActive(false);

        //listOfMagicScrolls[currentCount].SetActive(true);
    }

    public void Update()   
    {
        if (TutorialSteps.Instance.isStep4_Complete)
        {
            //to the right
            if (Finch.FinchInput.GetTouchpadEvent(Node, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeLeft))
            {
                //if (currentCount + 1 >= listOfMagicScrolls.Count) currentCount = 0;
                //else currentCount++;

                //for (var i=0; i< listOfMagicScrolls.Count; i++)
                //    listOfMagicScrolls[i].SetActive(false);

                //listOfMagicScrolls[currentCount].SetActive(true);


                currentCount = LightweightBookHelper.Instance.Progress; //дает текущую страницу -- 0 это первая страница, 1 -- вторая, 1.35 -- идет прогресс перелистывания (на 35% ближе ко второй странице), 
                if (TutorialDraw.Instance != null)
                    TutorialDraw.Instance.ShowMagicSpellInfoAnimation(Mathf.Clamp(Mathf.RoundToInt(currentCount + 1), 0, 3));
                LightweightBookHelper.Instance.NextPage(0.2f);
                //if (currentCount + 1 >= listOfMagicScrolls.Count) currentCount = 0;
                //else currentCount++;
            }

            //to the Left
            if (Finch.FinchInput.GetTouchpadEvent(Node, (Finch.Internal.TouchpadEvents)RingTouchpadEvents.SwipeRight))
            {
                //if (currentCount - 1 < 0) currentCount = listOfMagicScrolls.Count-1;
                //else currentCount--;

                //for (var i = 0; i < listOfMagicScrolls.Count; i++)
                //    listOfMagicScrolls[i].SetActive(false);

                //listOfMagicScrolls[currentCount].SetActive(true);


                currentCount = LightweightBookHelper.Instance.Progress; //дает текущую страницу
                if (TutorialDraw.Instance != null)
                    TutorialDraw.Instance.ShowMagicSpellInfoAnimation(Mathf.Clamp(Mathf.RoundToInt(currentCount - 1), 0, 3));
                LightweightBookHelper.Instance.PrevPage(0.2f);
            }
        }
    }
}
