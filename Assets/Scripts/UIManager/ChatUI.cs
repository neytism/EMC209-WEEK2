using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : NetworkBehaviour
{
    [SerializeField] private GameObject _chatHolder;
    
    [SerializeField] private RectTransform _chatHistoryHolder;
    [SerializeField] private GameObject _chatTextPrefab;
    
    
    [SerializeField] private Button _button;
    [SerializeField] private TMP_InputField _input;
    public event Action<string> OnMesageSent;

    private void Awake()
    {
        _button.onClick.AddListener(() =>
        {
            OnMesageSent?.Invoke(_input.text);
            _chatHolder.SetActive(false);
        });
        
        
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Return))
        {
            _chatHolder.SetActive(!_chatHolder.activeSelf);
            _input.text = "";
        }
    }
    
    [Rpc]
    public void RPC_AddToChatHistory(string text)
    {
        GameObject newChat = Instantiate(_chatTextPrefab, _chatHistoryHolder);
        newChat.GetComponent<TextMeshProUGUI>().text = text;

    }
}
