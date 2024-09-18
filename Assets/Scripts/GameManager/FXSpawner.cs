using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using GNW;
using UnityEngine;

public class FXSpawner : NetworkBehaviour
{
    [SerializeField] private ParticleSystem _hitFX;
    [SerializeField] private ParticleSystem _deathFX;

    private void Awake()
    {
        Health.OnHitEvent += RPC_SpawnHitFX;
        Health.OnDeathEvent += RPC_SpawnDeathFX;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SpawnHitFX(Vector3 pos)
    {
        if (_hitFX == null) return;

        Instantiate(_hitFX, pos, Quaternion.identity);

    }
        
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SpawnDeathFX(Vector3 pos)
    {
        if (_deathFX == null) return;

        var fxInstance = Instantiate(_deathFX, pos, Quaternion.identity);
        fxInstance.transform.SetParent(null); 
    }
}
