using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GNW.UIManager
{
    public class UIManager : MonoBehaviour
    {
        public static event Action<GameMode> OnHostButtonEvent;

        public static event Action<GameMode> OnClientButtonEvent;

        public static event Action OnFireButtonEvent;

        [SerializeField] private GameObject _startHolder;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;
        [SerializeField] private Button _fireButton;

        private void Start()
        {
            _hostButton.onClick.AddListener(() => OnHostButtonEvent?.Invoke(GameMode.Host));
            _clientButton.onClick.AddListener(() => OnClientButtonEvent?.Invoke(GameMode.Client));
            _fireButton.onClick.AddListener(() => OnFireButtonEvent?.Invoke());
        }

        private void OnEnable()
        {
            OnHostButtonEvent += HideStartButtons;
            OnClientButtonEvent += HideStartButtons;
        }

        private void HideStartButtons(GameMode m)
        {
            _startHolder.gameObject.SetActive(false);
        }
        
        
    }

}
