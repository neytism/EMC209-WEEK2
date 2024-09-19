using System;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatInputUI : MonoBehaviour
{
    [SerializeField] private GameObject _chatHolder;
    
    [SerializeField] private Button _button;
    [SerializeField] private TMP_InputField _input;
    public event Action<string> OnMesageSent;

    private ChatManager _chatManager;

    private void Awake()
    {
        _button.onClick.AddListener(() =>
        {
            OnMesageSent?.Invoke(_input.text);
            _chatHolder.SetActive(false);
        });
        
        _chatManager = FindObjectOfType<ChatManager>();
        
        if (_chatManager == null)
        {
            Debug.LogWarning("Chat Manager Don't exist.");
        }
        
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Return))
        {
            if (_chatHolder.activeSelf)
            {
                if (_input.text == String.Empty)
                {
                    _chatHolder.SetActive(false);
                    _input.DeactivateInputField();
                }
                else
                {
                    _button.onClick.Invoke();
                }
               
            }
            else
            {
                _chatHolder.SetActive(true);
                _input.ActivateInputField();
            }
            
            _input.Select();
            _input.text = "";
        }
    }

   
}