using System.Collections;
using System.Collections.Generic;
using Main.Scripts.PhotonAR;
using UnityEngine;

public class PhotonTransformViewARPlayer : PhotonTransformViewAR
{
    protected override void AwakeMethod()
    {
        base.AwakeMethod();
        
    }

    protected override Quaternion GetCurrentRotation()
    {
        return cacheTr.parent.rotation;
    }
    
}
