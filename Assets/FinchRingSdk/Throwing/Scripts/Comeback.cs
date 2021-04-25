using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Finch;

public class Comeback : MonoBehaviour
{
    private Vector3 startPos;
    private Quaternion startRot;
    public GameObject podl;
    private Rigidbody rig;

    private void Start()
    {
        Remember();
        rig = GetComponent<Rigidbody>();
    }


    void Update()
    {
        //if (FinchController.GetPressDown(Chirality.Any, RingElement.Trigger))
        //    podl.SetActive(false);

        //if (FinchController.GetPressDown(Chirality.Any, RingElement.AppButton))
        //{
        //    Comebacer();
        //}
    }

    public void Remember()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    public void Comebacer()
    {
        transform.position = startPos;
        transform.rotation = startRot;
        rig.angularVelocity = new Vector3();
        rig.velocity = new Vector3();
        podl.SetActive(true);
    }
}
