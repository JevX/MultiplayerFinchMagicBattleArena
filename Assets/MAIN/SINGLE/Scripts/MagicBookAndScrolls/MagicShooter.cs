using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MagicShooter : MonoBehaviour
{
    public MagicType shooterType;

    public abstract void ResetShooter();
    public abstract void BeginShooter();
    public abstract void CastShooter();

    public abstract void RegisterShooter(MagicShooter_Manager shooter);

}
