using System.Collections.Generic;
using Wand;
using UnityEngine;

namespace Wand
{
    public class MagicWandShooter : MonoBehaviour
    {
        public List<MagicShooter> shootersList;

        public Vector3 preciseBoxZone = Vector3.one;
        public float preciseBoxShift = 1;

        public Vector3 largeBoxZone = Vector3.one;
        public float largeBoxShift = 1;

        private MagicWandController MyWand => MagicWandController.Instance;

        public RaycastHit[] hitsArray;
   
        private void Awake()
        {
            foreach (MagicShooter shooter in shootersList)
            {
                shooter.RegisterShooter(this);
            }
        }

        public void ResetAllShooters()
        {
            foreach (MagicShooter shooter in shootersList)
            {
                shooter.ResetShooter();
            }
        }

        public void CastDone(MagicType magicType)
        {
            foreach (MagicShooter shooter in shootersList)
            {
                if (shooter.shooterType == magicType)
                {
                    shooter.ResetShooter();
                    shooter.CastShooter();
                }
            }
        }

        public void BeginCast(MagicType magicType)
        {
            foreach (MagicShooter shooter in shootersList)
            {
                if (shooter.shooterType == magicType)
                {
                    shooter.ResetShooter();
                    shooter.BeginShooter();
                }
            }
        }

        public void CastPreciseBox()
        {
            hitsArray = Physics.BoxCastAll(MyWand.wandEndPosition.position + (MyWand.wandEndPosition.forward * preciseBoxShift), preciseBoxZone, MyWand.wandEndPosition.forward, MyWand.wandEndPosition.rotation);
        }

        public void CastLargeBox()
        {
            hitsArray = Physics.BoxCastAll(MyWand.wandEndPosition.position + (MyWand.wandEndPosition.forward * largeBoxShift), largeBoxZone, MyWand.wandEndPosition.forward, MyWand.wandEndPosition.rotation);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(MyWand.wandEndPosition.position + (MyWand.wandEndPosition.forward * preciseBoxShift), preciseBoxZone * 2);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(MyWand.wandEndPosition.position + (MyWand.wandEndPosition.forward * largeBoxShift), largeBoxZone * 2);
        }
    }
}
