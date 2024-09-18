using System;
using Fusion;
using GNW.PlayerController;
using UnityEngine;
using UnityEngine.UI;

namespace GNW
{
    public class Health : NetworkBehaviour
    {
        private int _maxHealth = 60;
        [SerializeField] private Image _bar;
        [Networked] private int NetworkedCurrentHealth { get; set; }

        private Player _currentPlayer;

        private void Start()
        {
            NetworkedCurrentHealth = _maxHealth;
            _currentPlayer = GetComponent<Player>();
            _currentPlayer.OnTakeDamageEvent += TakeHealthDamage;
        }

        private void TakeHealthDamage(int dmg)
        {
            if (HasStateAuthority)
            {
                NetworkedCurrentHealth -= dmg;
                RPC_UpdateHealthBar(NetworkedCurrentHealth);    
            }
        }

        
        private void UpdateHealthBar()
        {
            _bar.fillAmount = (float)NetworkedCurrentHealth / (float)_maxHealth;
        }
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_UpdateHealthBar(int updatedHealth)
        {
            NetworkedCurrentHealth = updatedHealth; // Update the current health on all clients
            UpdateHealthBar(); // Update the UI health bar
        }
    }
}
