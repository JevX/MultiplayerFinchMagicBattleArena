using UnityEngine;

namespace MAIN.Scripts.Tests
{
    public class PlayerTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Spell")) ConfusePlayer(other.gameObject.name);
        }

        private void ConfusePlayer(string nameGO)
        {
            print($"ConfusePlayer {nameGO}");
        }
    }
}
