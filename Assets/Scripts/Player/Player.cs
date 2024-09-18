using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;
using GNW.InputData;
using GNW.Projectile;
using TMPro;

namespace GNW.PlayerController
{
    public class Player : NetworkBehaviour, ICombat
    {
        public static event Action<bool> OnFireCooldownEvent;
        
        [Networked] public string PlayerName { get; set; }
        [Networked] public Color PlayerColor { get; set; }
        [SerializeField] private float speed = 2f;
        
        [SerializeField] private BulletProjectile _bulletPrefab;
        [SerializeField] private float _fireRate = 0.1f;
        [Networked] private TickTimer FireRateTT { get; set; }
        private bool _canShoot = true;
        
        private Vector3 _firePoint = Vector3.forward * 2;
        
        private NetworkCharacterController _cc;
        private Renderer _ren;
        private GNW.GameManager.GameManager _gm;

        private bool _initInfoSynced = false;

        public event Action<int> OnTakeDamageEvent; 
        public event Action OnPlayerSpawnEvent;
        

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
                PlayerColor = new Color(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    1f
                );
                
                OnPlayerSpawnEvent?.Invoke();

            }
            
        }

        private void UpdateInfo()
        {
            if (_ren != null && !_initInfoSynced)
            {
                _ren.material.color = PlayerColor;
            }
        }

        public override void Render()
        {
            UpdateInfo();
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
            
            if (HasInputAuthority && FireRateTT.ExpiredOrNotRunning(Runner) != _canShoot)
            {
                _canShoot = FireRateTT.ExpiredOrNotRunning(Runner);
                OnFireCooldownEvent?.Invoke(_canShoot); 
            }
            
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


        public void TakeDamage(int dmg)
        {
            OnTakeDamageEvent?.Invoke(dmg);
        }
    }
}

