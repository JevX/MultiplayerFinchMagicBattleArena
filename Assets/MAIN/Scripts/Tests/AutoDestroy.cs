using System.Collections;
using UnityEngine;

namespace MAIN.Scripts.Tests
{
    public class AutoDestroy : MonoBehaviour
    {
        public float secToDestroy = 2;
    
        void Start()
        {
            StartCoroutine(DestroyThis());
        }

        private IEnumerator DestroyThis()
        {
            yield return new WaitForSeconds(secToDestroy);
            Destroy(gameObject);
        }
    }
}
