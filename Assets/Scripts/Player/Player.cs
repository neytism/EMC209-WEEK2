using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;
using GNW.InputData;
using GNW.Projectile;
using GNW.GameManager;

namespace GNW.PlayerController
{
    public class Player : NetworkBehaviour
    {
        [Networked] public Color playerColor { get; set; }
        [SerializeField] private float speed = 2f;
        
        [SerializeField] private BulletProjectile _bulletPrefab;
        [SerializeField] private float _fireRate = 0.1f;
        [Networked] private TickTimer FireRateTT { get; set; }
        
        private Vector3 _firePoint = Vector3.forward * 2;
        
        private NetworkCharacterController _cc;
        private Renderer _ren;
        private GNW.GameManager.GameManager _gm;

        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterController>();
            _ren = GetComponentInChildren<Renderer>();
            _gm = FindObjectOfType<GNW.GameManager.GameManager>();
        }

        public override void Spawned()
        {
            if (HasStateAuthority) // only the server set the color
            {
                playerColor = new Color(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    1f
                );
            }
            
            UpdateColor();
        }

        private void UpdateColor()
        {
            if (_ren != null)
            {
                _ren.material.color = playerColor;
            }
        }

        public override void Render()
        {
            UpdateColor();
        }

        public override void FixedUpdateNetwork()
        {
           MoveHandler();
           ShootHandler();
        }

        public void MoveHandler()
        {
            if (!GetInput(out NetworkInputData data)) return;
            
            data.Direction.Normalize();
            _cc.Move(speed * data.Direction * Runner.DeltaTime);
            
            if (data.buttons.IsSet(NetworkInputData.JUMPBUTTON) && _cc.Grounded) _cc.Jump();

            
        }

        public void ShootHandler()
        {
            if (!GetInput(out NetworkInputData data)) return;
            
            //if (!HasInputAuthority || !FireRateTT.ExpiredOrNotRunning(Runner)) return;
            if (!FireRateTT.ExpiredOrNotRunning(Runner)) return;
            
            if (data.Direction.sqrMagnitude > 0)
            {
                _firePoint = data.Direction * 2;
            }else
            {
                _firePoint = transform.forward * 2; 
            }

            if (data.buttons.IsSet(NetworkInputData.SHOOTBUTTON))
            {
                FireRateTT = TickTimer.CreateFromSeconds(Runner, _fireRate);
                
                if (Runner.IsServer)
                {
                    Runner.Spawn(_bulletPrefab, transform.position + _firePoint, Quaternion.LookRotation(_firePoint), Object.InputAuthority,
                        (runner, o) =>
                        {
                            Player targetPlayer = FindNearestPlayer();
                            o.GetComponent<BulletProjectile>()?.Init(targetPlayer);
                        });
                }
            }
        }

        private Player FindNearestPlayer()
        {
            Player nearestPlayer = null;
            float nearestDistance = float.MaxValue;

            foreach (var kvp in _gm.SpawnedPlayers)
            {
                NetworkObject networkObject = kvp.Value;
                if (networkObject.TryGetComponent(out Player player) && player != this)
                {
                    float distance = Vector3.Distance(transform.position, player.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestPlayer = player;
                    }
                }
            }

            return nearestPlayer;
        }


        
        
    }
}

