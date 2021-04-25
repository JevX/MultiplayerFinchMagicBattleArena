using System;
using Photon.Pun;

namespace Main.Scripts
{
    public class GameManager:MonoBehaviourPunCallbacks
    {
        public static GameManager Instance = null;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
    }
}