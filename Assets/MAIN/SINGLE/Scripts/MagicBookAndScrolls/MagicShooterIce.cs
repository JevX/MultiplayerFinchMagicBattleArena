using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MagicShooterIce : MagicShooter
{
    public ParticleSystem particlesOnCast;
    public ParticleSystem particlesOnBegin;
    public MMMiniObjectPooler bulletsPool;

    public MagicShooter_Manager myShooter;

    public override void BeginShooter()
    {
        particlesOnBegin.Play();
    }

    public override void CastShooter()
    {
        particlesOnCast.Play();

        myShooter.CastLargeBox();

        myShooter.hitsArray = myShooter.hitsArray.OrderBy((d) => (d.point - transform.position).sqrMagnitude).ToArray();

        if (myShooter.hitsArray.Length > 0)
        {
            foreach (RaycastHit hittedObject in myShooter.hitsArray)
            {
                if (TargetsManager.Instance.CheckIfTargetCanBeDamaged(hittedObject.collider.gameObject))
                {
                    GameObject pooledIce = bulletsPool.GetPooledGameObject();

                    if (pooledIce != null)
                    {
                        pooledIce.transform.position = hittedObject.collider.gameObject.transform.position + Vector3.up * 3f;
                        pooledIce.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        pooledIce.SetActive(true);
                    }

                    break;
                }
            }
        }
    }

    public override void RegisterShooter(MagicShooter_Manager shooter)
    {
        myShooter = shooter;
    }

    public override void ResetShooter()
    {
        particlesOnCast.Stop();
        particlesOnBegin.Stop();
    }
}
