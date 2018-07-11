using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.Events;

public class TableUIPanel : BasePanel
{
    [SerializeField] private ScrollRect scrollViewChat;
    [SerializeField] private Text chatText;
    [SerializeField] private InputField chatEntry;
    [SerializeField] private Button send;
    [SerializeField] private Button disconnect;
    [SerializeField] private TextMeshProUGUI statusBarText;

    private bool focusedRequested = true;

    public string GetChatEntry()
    {
        return chatEntry.text;
    }

    public void SetChatEntry(string pEntry)
    {
        chatEntry.text = pEntry;
        focusedRequested = true;
    }

    public void AddChatLine(string pChatLine)
    {
        chatText.text += pChatLine + "\n";
        scrollViewChat.verticalNormalizedPosition = 0;
    }

    public void SetStatusText(string text)
    {
        statusBarText.text = text;
    }
    
    public void ClearAllText()
    {
        chatText.text = "";
        statusBarText.text = "";
    }

    public void RegisterButtonSendClickAction(UnityAction pAction)
    {
        send.onClick.AddListener(pAction);
        chatEntry.onEndEdit.AddListener(value => pAction());
        focusedRequested = true;
    }

    public void RegisterButtonDisconnectClickAction(UnityAction pAction)
    {
        disconnect.onClick.AddListener(pAction);
    }

    public void UnregisterButtonSendClickActions()
    {
        send.onClick.RemoveAllListeners();
        chatEntry.onEndEdit.RemoveAllListeners();
    }

    public void UnregisterButtonDisconnectClickActions()
    {
        disconnect.onClick.RemoveAllListeners();
    }
    
    void Update()
    {
        if (focusedRequested)
        {
            chatEntry.ActivateInputField();
            chatEntry.Select();
            focusedRequested = false;
        }
    }
}