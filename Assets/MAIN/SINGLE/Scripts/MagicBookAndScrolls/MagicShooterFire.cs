using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagicShooterFire : MagicShooter
{
    public MMMiniObjectPooler bulletsPool;

    public MagicShooter_Manager myShooter;

    public Rigidbody carriedObject;
    Coroutine objectChecker;

    public float staticThrowForce = 3f;
    public float guidedVelocityModifier = 2f;
    public float minimalVelocityForce = 1f;

    public override void BeginShooter()
    {
        GameObject pooledFireball = bulletsPool.GetPooledGameObject();
        if (pooledFireball != null)
        {
            ThrowCarriedObject();

            pooledFireball.transform.position = myShooter.myWand.wandEndPosition.position;
            pooledFireball.transform.parent = myShooter.myWand.wandEndPosition;
            carriedObject = pooledFireball.GetComponent<Rigidbody>();
            carriedObject.isKinematic = true;

            pooledFireball.SetActive(true);

            objectChecker = StartCoroutine(CheckObjectExistence());

            Debug.Log("firebal started");

        }
    }

    public override void CastShooter()
    {
        Debug.Log("firebal throwed");
        ThrowCarriedObject();
    }

    /// <summary>
    /// Кидает удерживаемый объект
    /// </summary>
    public void ThrowCarriedObject()
    {
        if (carriedObject)
        {
            if (!carriedObject.gameObject.activeInHierarchy)
            {
                carriedObject.transform.parent = null;
                carriedObject = null;
                return;
            }

            Debug.Log("Object throwed");

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

            StopCoroutine(objectChecker);
            carriedObject = null;
        }
    }

    IEnumerator CheckObjectExistence() //Может его вообще единожды запускать? Если мы объект выбросили, он уже Null и Throw его проигнорирует
    {
        while (carriedObject != null && carriedObject.gameObject.activeInHierarchy)
        {
            yield return null;
        }
        ThrowCarriedObject();
    }

    public override void RegisterShooter(MagicShooter_Manager shooter)
    {
        myShooter = shooter;
    }

    public override void ResetShooter()
    {

        ThrowCarriedObject();
    }

}
