using System.Collections;
using System.Collections.Generic;
using Fusion;
using GNW.PlayerController;
using TMPro;
using UnityEngine;

public class NameTag : NetworkBehaviour
{
    public TextMeshProUGUI nameTagText;

    public void UpdateNameTag(string n)
    {
        nameTagText.text = n;
    }
    
    private void Awake()
    {
        Player.OnPlayerSpawnEvent += RPC_UpdateNameTag;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateNameTag(string name,NetworkId playerId)
    {
        if (nameTagText == null) return;
        nameTagText.text = name;
        
        if (playerId == Object.Id)
        {
            nameTagText.text = name;
        }
    }
}
