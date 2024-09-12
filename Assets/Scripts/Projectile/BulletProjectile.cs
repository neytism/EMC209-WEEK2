using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace GNW.Projectile
{
    public class BulletProjectile : NetworkBehaviour
    {
        [SerializeField] private float _bulletSpeed = 10f;
        [SerializeField] private float _life = 5f;
        [Networked] private TickTimer LifeTT { get; set; }
        [Networked] private Color _bulletColor { get; set; }
        
        [SerializeField] private Renderer _ren;
        
        private PlayerController.Player _targetPlayer;

        public void Init(PlayerController.Player targetPlayer = null)
        {
            LifeTT = TickTimer.CreateFromSeconds(Runner, _life);
            _targetPlayer = targetPlayer;
        }

        public override void FixedUpdateNetwork()
        {
            if (LifeTT.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
            else
            {
                //ensures movement is the same across all networked players
                if (Object.HasStateAuthority)
                {
                    if (_targetPlayer != null)
                    {
                        Vector3 direction = (_targetPlayer.transform.position - transform.position).normalized;
                        transform.position += direction * _bulletSpeed * Runner.DeltaTime;
                    }
                    else
                    {
                        transform.position += transform.forward * _bulletSpeed * Runner.DeltaTime;
                    }
                    
                    float? lifePercentage = LifeTT.RemainingTime(Runner) / _life;
                    if (lifePercentage != null)
                    {
                        Color newColor = Color.Lerp(Color.red, Color.white, lifePercentage.Value);
                        _bulletColor = newColor;
                    }

                    
                }
            }
            
        }
        
        public override void Render()
        {
            _ren.material.color = _bulletColor;
        }
    }
}

