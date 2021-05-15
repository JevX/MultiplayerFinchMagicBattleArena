using System.Linq;
using MoreMountains.Feedbacks;
using UnityEngine;
using Wand;

namespace Wand
{
    public class MagicShooterElectro : MagicShooter
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
                    if (TargetsManager.Instance.DamageRegisteredTarget(1, MagicType.Electro, hittedObject.collider.gameObject))
                    {
                        GameObject pooledElectro = bulletsPool.GetPooledGameObject();
                        if (pooledElectro != null)
                        {
                            pooledElectro.transform.position = hittedObject.collider.gameObject.transform.position;
                            pooledElectro.SetActive(true);
                            pooledElectro.GetComponent<ParticleSystem>().Play();
                        }
                        break;
                    }
                }
            }
        }

        public override void ResetShooter()
        {
            particlesOnCast.Stop();
            particlesOnBegin.Stop();
        }

        public override void RegisterShooter(MagicWandShooter shooter)
        {
            myShooter = shooter;
        }


    }
}
