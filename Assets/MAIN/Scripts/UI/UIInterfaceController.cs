using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MAIN.Scripts.UI
{
    public class UIInterfaceController : MonoBehaviour
    {
        [SerializeField] private List<InterfaceUI> interfaces = null;
        [Header("Найти всё (не рекомендуется)")]
        [SerializeField] private bool isFindAllInterfaces;
        public static UIInterfaceController Instance = null;
        
        private void Awake()
        {
            if (Instance != null) Destroy(Instance);
            
            Instance = this;

            if (isFindAllInterfaces) interfaces = Resources.FindObjectsOfTypeAll<InterfaceUI>().ToList();
        }

        public void OpenInterface(InterfaceType type)
        {
            HideAll();
            InterfaceUI interfaceUI = interfaces.FirstOrDefault(t => t.InterfaceType == type);
                if (interfaceUI == null) Debug.LogError($"не найден тип интерфейса {type}");
                else interfaceUI.Open();
        }

        private void HideAll()
        {
            interfaces.ForEach(t =>
            {
                if (t.gameObject.activeSelf) t.Hide();
            });
        }
    }
}
