using System.Linq;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Wand
{
    public class MagicShooterIce : MagicShooter
    {
        public ParticleSystem particlesOnCast;
        public ParticleSystem particlesOnBegin;
        public MMMiniObjectPooler bulletsPool;

        public MagicWandShooter myShooter;

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

        public override void RegisterShooter(MagicWandShooter shooter)
        {
            myShooter = shooter;
        }

        public override void ResetShooter()
        {
            particlesOnCast.Stop();
            particlesOnBegin.Stop();
        }
    }
}
