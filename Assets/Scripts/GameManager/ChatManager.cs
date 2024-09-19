using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    

    [SerializeField] private RectTransform _chatHistoryHolder;
    [SerializeField] private GameObject _chatTextPrefab;
    
    // public override void Spawned()
    // {
    //     Debug.Log("Chat Manager Spawned");
    // }
    //
    // [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    // public void RPC_AddToChatHistory(string text)
    // {
    //     GameObject newChat = Instantiate(_chatTextPrefab, _chatHistoryHolder);
    //     newChat.GetComponent<TextMeshProUGUI>().text = text;
    // }

    public void InstantiateChat(string text)
    {
        GameObject newChat = Instantiate(_chatTextPrefab, _chatHistoryHolder);
        newChat.GetComponent<TextMeshProUGUI>().text = text;
    }
}
