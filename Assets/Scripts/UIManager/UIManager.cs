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
        public static event Action<GameMode> OnStartGameButtonEvent;
        
        [SerializeField] private GameObject _startHolder;
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _clientButton;
        [SerializeField] private Button _autoStartutton;
        [SerializeField] private TextMeshProUGUI _fireCooldownUI;
        [SerializeField] private TMP_InputField _inputName;

        private void Awake()
        {
            _hostButton.onClick.AddListener(() => StartGame(GameMode.Host));
            _clientButton.onClick.AddListener(() => StartGame(GameMode.Client));
            _autoStartutton.onClick.AddListener(() => StartGame(GameMode.AutoHostOrClient));
            
        }

        private void OnEnable()
        {
            OnStartGameButtonEvent += HideStartButtons;
            Player.OnFireCooldownEvent += UpdateFireCooldownUI;
        }
        
        private void StartGame(GameMode mode)
        {
            OnStartGameButtonEvent?.Invoke(mode); 
    
            if (mode == GameMode.Host)
            {
                var player = FindObjectOfType<Player>();
                player.SetPlayerName(GetPlayerName());
            }
            else if (mode == GameMode.Client)
            {
                var player = FindObjectOfType<Player>();
                player.SetPlayerName(GetPlayerName());
            }
        }

        
        private void HideStartButtons(GameMode m)
        {
            _startHolder.gameObject.SetActive(false);
        }
        
        public string GetPlayerName()
        {
            return _inputName.text;
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
