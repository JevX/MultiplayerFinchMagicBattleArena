using UnityEngine;

namespace Wand
{
    public abstract class MagicShooter : MonoBehaviour
    {
        public MagicType shooterType;
        public MagicWandController MyWand => MagicWandController.Instance;
        public abstract void ResetShooter();
        public abstract void BeginShooter();
        public abstract void CastShooter();

        public abstract void RegisterShooter(MagicWandShooter shooter);

    }
}
