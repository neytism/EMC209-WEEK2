using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;
using GNW.InputData;
using GNW.Projectile;
using TMPro;
using Unity.VisualScripting;

namespace GNW.PlayerController
{
    public class Player : NetworkBehaviour, ICombat
    {
        public static event Action<bool> OnFireCooldownEvent;
        public static event Action OnWinEvent;
        public static event Action OnLoseEvent;
        
        public static event Action<bool> OnGameFinishEvent;
        
        [Networked] private NetworkString<_8> PlayerName { get; set; }
        public TextMeshProUGUI nameTagText;
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
        
        private ChatInputUI _chatInputUI;
        private ChatManager _chatManager;

        public event Action<int> OnTakeDamageEvent; 

        private ChangeDetector _changeDetector;

        [SerializeField] private Animator _anim;
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");

        private bool _isWinner = false;

        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterController>();
            _ren = GetComponentInChildren<Renderer>();
            _gm = FindObjectOfType<GNW.GameManager.GameManager>();
            
            _chatInputUI = FindObjectOfType<ChatInputUI>();
            if (_chatInputUI != null)
            {
                _chatInputUI.OnMesageSent += SendChatInputMessage;
            }
            else
            {
                Debug.LogWarning("No Chat Found");
            }

            _chatManager = FindObjectOfType<ChatManager>();
            if (_chatManager == null)
            {
                Debug.LogWarning("No ChatManager Found");
            }
        }

        public override void Spawned()
        {
           
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            
            InitializeLocalPlayer();

            
        }

        private void InitializeLocalPlayer()
        {
            if (HasStateAuthority) // only the server set the color
            {
                PlayerColor = new Color(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    1f
                );
                
            }
            
            if (Runner.LocalPlayer == Object.InputAuthority)
            {
                var nickname = GlobalManagers.Instance.GameManager.LocalPlayerName;
                RPC_SetName(nickname);
                Debug.Log("This Player Spawned " + PlayerName + " " + Object.InputAuthority.PlayerId);
            }
            else
            {
                Debug.Log("Other Player Spawned " + PlayerName + " " + Object.InputAuthority.PlayerId);
                SendChatInputMessage(PlayerName + " Joined.");
                SetPlayerNickNameText(PlayerName);
            }

        }
        
        private void UpdateInfo()
        {
            if (_ren != null && !_initInfoSynced)
            {
                _ren.material.color = PlayerColor;
                _initInfoSynced = true;
            }
        }
        
        public override void FixedUpdateNetwork()
        {
            MoveHandler();
            ShootHandler();
        }

        public override void Render()
        {
            UpdateInfo();

            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(PlayerName):
                        SetPlayerNickNameText(PlayerName);
                        break;
                }
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        private void RPC_SetName(string nameStr)
        {
            PlayerName = nameStr;
        }
        
        private void SetPlayerNickNameText(NetworkString<_8> nameStr)
        {
            nameTagText.text = nameStr + " " + Object.InputAuthority.PlayerId;
        }
        
        public void MoveHandler()
        {
            if (!GetInput(out NetworkInputData data)) return;
            
            data.Direction.Normalize();
            _cc.Move(speed * data.Direction * Runner.DeltaTime);

            if (_anim)
            {
                _anim.SetBool(IsMoving, data.Direction != Vector3.zero);
            }


            if (data.buttons.IsSet(NetworkInputData.JUMPBUTTON) && _cc.Grounded)
            {
                _cc.Jump();
            }
        }

        public void ShootHandler()
        {
            if (!GetInput(out NetworkInputData data)) return;
            
            if (HasInputAuthority && FireRateTT.ExpiredOrNotRunning(Runner) != _canShoot)
            {
                _canShoot = FireRateTT.ExpiredOrNotRunning(Runner);
                OnFireCooldownEvent?.Invoke(_canShoot); 
            }
            
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
                Debug.Log(PlayerName + " " + Object.InputAuthority.PlayerId + " Shoots!");
                
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
        
        private void SendChatInputMessage(string message)
        {
            if (Runner.LocalPlayer == Object.InputAuthority)
            {
                RPC_SendChat(PlayerName + ": " + message);
            }
        }
        
        [Rpc]
        public void RPC_SendChat(string message)
        {
            _chatManager.InstantiateChat(message);
            //_chatManager.RPC_AddToChatHistory(message);
        }

        public void ReachGoal()
        {
            
                RPC_SendWinner();
                RPC_Test();
        }
        
//[Rpc]
        public void RPC_SendWinner()
        {
            OnWinEvent?.Invoke();
        }
        
        [Rpc(RpcSources.InputAuthority, RpcTargets.Proxies)]
        public void RPC_Test()
        {
            OnLoseEvent?.Invoke();
        }

        
        public void WinnerChecker(bool isWin)
        {
            OnGameFinishEvent?.Invoke(isWin);
        }
        
       

    }
}