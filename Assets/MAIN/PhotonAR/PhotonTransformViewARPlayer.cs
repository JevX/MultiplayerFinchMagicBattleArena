using UnityEngine;

namespace Main.PhotonAR
{
    public class PhotonTransformViewARPlayer : PhotonTransformViewAR
    {
        [SerializeField] private string nameCameraOnScene = "AR Camera";
    
        private Transform mainObjectAnchor = null;
        protected override void AwakeMethod()
        {
            base.AwakeMethod();
            mainObjectAnchor = GameObject.Find(nameCameraOnScene).transform;
        }

        protected override Quaternion GetCurrentRotation()
        {
            return mainObjectAnchor?.rotation ?? cacheTr.parent.rotation;
        }
    
    }
}
