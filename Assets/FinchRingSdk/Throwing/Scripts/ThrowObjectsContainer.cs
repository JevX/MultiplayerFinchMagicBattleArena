using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObjectsContainer : MonoBehaviour
{
    public Comeback[] ThrowObjects;

    public void Remember()
    {
        foreach (var obj in ThrowObjects)
        {
            obj.Remember();
        }
    }

    public void Comeback()
    {
        foreach (var obj in ThrowObjects)
        {
            obj.Comebacer();
        }
    }
}
