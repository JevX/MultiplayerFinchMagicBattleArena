using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class TutorialDraw : MMSingleton<TutorialDraw>
{
    public Animator tutorialDrawAnimator;

    public void ShowMagicSpellInfoAnimation(int pageMagicBook)
    {
        switch (pageMagicBook)
        {
            case 0://1 страница
                tutorialDrawAnimator.Play("DrawFire");
                break;
            case 1://2 страница                
                tutorialDrawAnimator.Play("DrawElectro");
                break;
            case 2:
                tutorialDrawAnimator.Play("DrawIce");
                break;
            case 3:
                tutorialDrawAnimator.Play("DrawKinetic");
                break;
            default:
                break;
        }
    }
}
