using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagicShooterKinetic : MagicShooter, MMEventListener<MMGameEvent>
{
    public ParticleSystem particlesOnHoldObject;
    public ParticleSystem particlesOnSearchObject;
    public MMMiniObjectPooler bulletsPool;

    public MagicShooter_Manager myShooter;

    public Rigidbody carriedObject;
    Coroutine objectChecker;

    public float staticThrowForce = 3f;
    public float guidedVelocityModifier = 2f;
    public float minimalVelocityForce = 1f;
    public float lerpObjectGuideSpeed = 1f;
    public float vacuumDistance = 1f;
    public float kineticForce = 1f;

    Coroutine mainCoroutine;

    [FMODUnity.EventRef]
    public string FMODKineticHoldObject;
    [FMODUnity.EventRef]
    public string FMODKineticPushObject;
    [FMODUnity.EventRef]
    public string FMODKineticSearchObject;

    private FMOD.Studio.EventInstance FMODInstance;

    public override void BeginShooter()
    {

    }

    public override void CastShooter()
    {
        if (mainCoroutine != null)
        {
            StopCoroutine(mainCoroutine);
            mainCoroutine = null;
            particlesOnSearchObject.Stop();
            ThrowCarriedObject();
            return;
        }

        if (!carriedObject)
        {
            mainCoroutine = StartCoroutine(SearchObject());
        }
        else
        {
            ThrowCarriedObject();
        }
    }

    public override void RegisterShooter(MagicShooter_Manager shooter)
    {
        myShooter = shooter;
    }

    public override void ResetShooter()
    {
        if (mainCoroutine != null)
        {
            StopCoroutine(mainCoroutine);
            mainCoroutine = null;
        }
        particlesOnSearchObject.Stop();
        particlesOnHoldObject.Stop();
        FMODInstance.release();
        FMODInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    IEnumerator GuideObjectToWand(float shiftDistance)
    {
        Vector3 objectOriginalPosition = carriedObject.transform.position;
        carriedObject.transform.parent = null;
        float pos = 0;
        while (pos < 1)
        {
            carriedObject.transform.position = Vector3.Lerp(objectOriginalPosition, myShooter.myWand.wandEndPosition.position + (myShooter.myWand.wandEndPosition.forward * shiftDistance), pos);
            pos += Time.deltaTime * lerpObjectGuideSpeed;
            yield return null;
        }
        carriedObject.transform.parent = myShooter.myWand.wandEndPosition;
        carriedObject.transform.position = myShooter.myWand.wandEndPosition.position + (myShooter.myWand.wandEndPosition.forward * shiftDistance);
        yield return null;
    }

    IEnumerator CheckObjectExistence()
    {
        while (carriedObject != null && carriedObject.gameObject.activeInHierarchy)
        {
            yield return null;
        }
        ThrowCarriedObject();
    }


    /// <summary>
    /// Назначает объект как удерживаемый на конце палочки
    /// </summary>
    /// <param name="objectToCarry"></param>
    public void CarryTargetedObject(Collider objectToCarry)
    {
        ThrowCarriedObject();

        FMODInstance.release();
        FMODInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        FMODInstance = FMODUnity.RuntimeManager.CreateInstance(FMODKineticHoldObject);
        FMODInstance.start();

        carriedObject = objectToCarry.attachedRigidbody;

        if (carriedObject == null)
            return;

        Vector3 closestPoint = carriedObject.ClosestPointOnBounds(myShooter.myWand.wandEndPosition.position);
        float shiftDistance = Vector3.Distance(closestPoint, carriedObject.transform.position);

        carriedObject.isKinematic = true;

        EnchantmentGlyph glyph = carriedObject.gameObject.GetComponent<EnchantmentGlyph>();
        if (glyph != null)
        {
            glyph.GlyphCarried();
        }

        particlesOnHoldObject.Play();

        objectChecker = StartCoroutine(CheckObjectExistence()); 
        mainCoroutine = StartCoroutine(GuideObjectToWand(shiftDistance));
    }

    /// <summary>
    /// Кидает удерживаемый объект
    /// </summary>
    public void ThrowCarriedObject()
    {
        if (carriedObject)
        {
            particlesOnHoldObject.Stop();

            if (mainCoroutine != null)
            {
                StopCoroutine(mainCoroutine);
                mainCoroutine = null;
            }

            if (!carriedObject.gameObject.activeInHierarchy)
            {
                carriedObject.transform.parent = null;
                carriedObject.isKinematic = false;
                carriedObject = null;

                return;
            }

            FMODInstance.release();
            FMODInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            FMODUnity.RuntimeManager.PlayOneShot(FMODKineticPushObject, transform.position);

            carriedObject.transform.parent = null;
            carriedObject.isKinematic = false;

            myShooter.CastLargeBox();
            myShooter.hitsArray = myShooter.hitsArray.OrderBy((d) => (d.point - transform.position).sqrMagnitude).ToArray();

            bool guidedVelocity = false;
            Vector3 guidedDirection = Vector3.zero;
            foreach (RaycastHit hit in myShooter.hitsArray)
            {
                if (TargetsManager.Instance.CheckIfTargetCanBeDamaged(hit.collider.gameObject))
                {
                    guidedDirection = hit.collider.ClosestPointOnBounds(myShooter.myWand.transform.position) - myShooter.myWand.transform.position;
                    guidedVelocity = true;
                    break;
                }
            }

            if (guidedVelocity)
            {
                carriedObject.velocity = guidedDirection.normalized * staticThrowForce * guidedVelocityModifier;
            }
            else
            {
                Vector3 sampledVelocity = MotionEventListener.Instance.GetMotionVelocity();
                carriedObject.velocity = sampledVelocity.magnitude > minimalVelocityForce ? sampledVelocity : myShooter.myWand.wandEndPosition.forward * staticThrowForce;
            }

            DamagingObject damageObj = carriedObject.gameObject.GetComponent<DamagingObject>();
            if (damageObj != null)
            {
                damageObj.MarkObjectAsThrowed();
            }

            carriedObject = null;
        }
    }


    IEnumerator SearchObject()
    {
        FMODInstance.release();
        FMODInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        FMODInstance = FMODUnity.RuntimeManager.CreateInstance(FMODKineticSearchObject);
        FMODInstance.start();

        particlesOnSearchObject.Play();

        while (carriedObject == null)
        {
            myShooter.CastPreciseBox();

            myShooter.hitsArray = myShooter.hitsArray.OrderBy((d) => (d.point - transform.position).sqrMagnitude).ToArray();

            if (myShooter.hitsArray.Length > 0)
            {
                bool objectFinded = false;
                foreach (RaycastHit hittedObject in myShooter.hitsArray)
                {
                    if (hittedObject.rigidbody != null)
                    {
                        if (hittedObject.distance > vacuumDistance)
                        {
                            hittedObject.rigidbody.AddForce(-myShooter.myWand.wandEndPosition.forward * kineticForce, ForceMode.Force);
                        }
                        else if (!objectFinded && !hittedObject.rigidbody.isKinematic)
                        {
                            CarryTargetedObject(hittedObject.collider);
                            objectFinded = true;
                        }
                    }

                }
            }

            yield return null;
        }

        particlesOnSearchObject.Stop();
    }

    public void OnMMEvent(MMGameEvent eventType)
    {
        if (eventType.EventName == StaticEvents.magicChanged.EventName)
        {
            ThrowCarriedObject();
        }
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
