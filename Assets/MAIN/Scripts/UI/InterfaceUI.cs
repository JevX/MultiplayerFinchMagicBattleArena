using UnityEngine;

namespace MAIN.Scripts.UI
{
    public class InterfaceUI : MonoBehaviour
    {
        [SerializeField] protected InterfaceType interfaceType;
        
        public InterfaceType InterfaceType => interfaceType;
        
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
