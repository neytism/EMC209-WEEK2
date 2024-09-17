using System;
using Fusion;
using GNW.PlayerController;
using UnityEngine;

namespace GNW
{
    public class Health : NetworkBehaviour
    {
        private int _maxHealth = 60;
        [Networked] private int _currentHealth { get; set; }

        private Player _currentPlayer;

        private void Start()
        {
            _currentHealth = _maxHealth;
            _currentPlayer = GetComponent<Player>();
            _currentPlayer.OnTakeDamageEvent += TakeHealthDamage;
        }

        private void TakeHealthDamage(int dmg)
        {
            _currentHealth -= dmg;
        }
    }
}
