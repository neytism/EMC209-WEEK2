using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using GNW.PlayerController;

namespace GNW.UIManager
{
    public class UIManager : MonoBehaviour
    {
        public static event Action<GameMode> OnHostButtonEvent;

        public static event Action<GameMode> OnClientButtonEvent;



        [SerializeField] private GameObject _startHolder;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;
        [SerializeField] private TextMeshProUGUI _fireCooldownUI;

        private void Start()
        {
            _hostButton.onClick.AddListener(() => OnHostButtonEvent?.Invoke(GameMode.Host));
            _clientButton.onClick.AddListener(() => OnClientButtonEvent?.Invoke(GameMode.Client));
            
        }

        private void OnEnable()
        {
            OnHostButtonEvent += HideStartButtons;
            OnClientButtonEvent += HideStartButtons;
            Player.OnFireCooldownEvent += UpdateFireCooldownUI;
        }

        private void HideStartButtons(GameMode m)
        {
            _startHolder.gameObject.SetActive(false);
        }
        
        private void UpdateFireCooldownUI(bool canShoot)
        {
            if (canShoot)
            {
                _fireCooldownUI.color = Color.green;
                _fireCooldownUI.text = "can shoot";
            }
            else
            {
                _fireCooldownUI.color = Color.red;
                _fireCooldownUI.text = "can't shoot";
            }
        }
        
    }

}
