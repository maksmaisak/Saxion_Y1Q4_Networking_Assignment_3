using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class TableChatPanel : BasePanel
{
    [SerializeField] private ScrollRect scrollViewChat;
    [SerializeField] private Text chatText;
    [SerializeField] private InputField chatEntry;
    [SerializeField] private Button send;
    [SerializeField] private Button disconnect;

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

    public void ClearAllText()
    {
        chatText.text = "";
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

    public void Update()
    {
        if (focusedRequested)
        {
            chatEntry.ActivateInputField();
            chatEntry.Select();
            focusedRequested = false;
        }
    }
}