using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ETrigState
{
    Undefined,

    Active,
    Selected,
    Disabled
}

public class UColorParticle : MonoBehaviour
{
    public GameObject[] Active;
    public GameObject[] Selected;
    public GameObject[] Disabled;

    public ETrigState State = ETrigState.Disabled;

    void Start()
    {
        
    }


    private ETrigState OldState = ETrigState.Undefined;
    void Update()
    {
        if(OldState!=State)
        {
            OldState = State;

            for (int i = 0; i < Active.Length; ++i)     Active[i].SetActive(false);
            for (int i = 0; i < Disabled.Length; ++i)   Disabled[i].SetActive(false);
            for (int i = 0; i < Selected.Length; ++i)   Selected[i].SetActive(false);

            for (int i = 0; i < Active.Length; ++i)     if (State == ETrigState.Active)     Active[i].SetActive(true);
            for (int i = 0; i < Disabled.Length; ++i)   if (State == ETrigState.Disabled)   Disabled[i].SetActive(true);
            for (int i = 0; i < Selected.Length; ++i)   if (State == ETrigState.Selected)   Selected[i].SetActive(true);
        }
    }
}
