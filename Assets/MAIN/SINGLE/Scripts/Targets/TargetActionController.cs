using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class TargetActionController : MonoBehaviour
{
    public bool playRagdoll=true;
    public GameObject textHealth;
    public GameObject DummyFull;
    public GameObject DummyBroken;
    public ParticleSystem Hit_FX;
    public Collider hitCollider;

    private Animator mAnimator;

    private int rand;
    // Start is called before the first frame update

    [FMODUnity.EventRef]
    public string FMODTargetHit1;
    [FMODUnity.EventRef]
    public string FMODTargetHit2;

    //private FMOD.Studio.EventInstance FMODInstance;

    List<Transform> childTransforms = new List<Transform>();
    Vector3[] startPositions;
    Quaternion[] startRotations;
    Transform[] startParents;

    void Start()
    {
        mAnimator = DummyFull.GetComponent<Animator>();

        foreach (Transform child in DummyBroken.transform)
        {
            childTransforms.Add(child);
        }
        startPositions = new Vector3[childTransforms.Count];
        startRotations = new Quaternion[childTransforms.Count];
        startParents = new Transform[childTransforms.Count];
        for (int i = 0; i < childTransforms.Count; i++)
        {
            startPositions[i] = childTransforms[i].localPosition;
            startRotations[i] = childTransforms[i].localRotation;
            startParents[i] = childTransforms[i].parent;
        }
    }    

    public void PlayRandomAnimationsHit()
    {
        rand = Random.Range(1, 3);
        if (rand == 1)
        {
            mAnimator.Play("Dummy_Hit1");
            FMODUnity.RuntimeManager.PlayOneShot(FMODTargetHit1, transform.position);
            //FMODInstance = FMODUnity.RuntimeManager.CreateInstance(FMODTargetHit1);
            //FMODInstance.start();
            //FMODInstance.release();
        }
        else if (rand == 2)
        {
            mAnimator.Play("Dummy_Hit2");
            FMODUnity.RuntimeManager.PlayOneShot(FMODTargetHit2, transform.position);
            //FMODInstance = FMODUnity.RuntimeManager.CreateInstance(FMODTargetHit1);
            //FMODInstance.start();
            //FMODInstance.release();
        }
    }

    public void PlayParticleHitFx()
    {
        Hit_FX.Play();
    }

    public void KillTarget()
    {
        hitCollider.enabled = false;

        if (playRagdoll)
        {
            DummyFull.SetActive(false);
            DummyBroken.SetActive(true);
        }
        else
        {
            mAnimator.Play("Dummy_Death");
            FMODUnity.RuntimeManager.PlayOneShot(FMODTargetHit1, transform.position);
        }
    }

    public void ResetDummy()
    {
        for (int i = 0; i < childTransforms.Count; i++)
        {
            childTransforms[i].localPosition = startPositions[i];
            childTransforms[i].localRotation = startRotations[i];
            childTransforms[i].parent = startParents[i];
        }

        hitCollider.enabled = true;

        if (playRagdoll)
        {
            DummyBroken.SetActive(false);
            DummyFull.SetActive(true);
        }
        else
            mAnimator.Play("Dummy_Idle1");
    }

}
