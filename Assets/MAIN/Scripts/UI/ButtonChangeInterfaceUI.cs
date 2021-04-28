using System;
using UnityEngine;
using UnityEngine.UI;

namespace MAIN.Scripts.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonChangeInterfaceUI : MonoBehaviour
    {
        [SerializeField] private InterfaceType next;

        private Button _button = null;
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OpenInterface);
        }

        private void OpenInterface()
        {
            UIInterfaceController.Instance?.OpenInterface(next);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OpenInterface);
        }
    }
}
