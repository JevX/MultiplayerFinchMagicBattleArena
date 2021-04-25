using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using System.Linq;

public class MagicShooter_Manager : MonoBehaviour
{
    public List<MagicShooter> shootersList;

    public Vector3 preciseBoxZone = Vector3.one;
    public float preciseBoxShift = 1;

    public Vector3 largeBoxZone = Vector3.one;
    public float largeBoxShift = 1;

    public MagicWand myWand;

    
    public RaycastHit[] hitsArray;
   
    private void Awake()
    {
        myWand = GetComponent<MagicWand>();
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
        hitsArray = Physics.BoxCastAll(myWand.wandEndPosition.position + (myWand.wandEndPosition.forward * preciseBoxShift), preciseBoxZone, myWand.wandEndPosition.forward, myWand.wandEndPosition.rotation);
    }

    public void CastLargeBox()
    {
        hitsArray = Physics.BoxCastAll(myWand.wandEndPosition.position + (myWand.wandEndPosition.forward * largeBoxShift), largeBoxZone, myWand.wandEndPosition.forward, myWand.wandEndPosition.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(myWand.wandEndPosition.position + (myWand.wandEndPosition.forward * preciseBoxShift), preciseBoxZone * 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(myWand.wandEndPosition.position + (myWand.wandEndPosition.forward * largeBoxShift), largeBoxZone * 2);
    }
}
