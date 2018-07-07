using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ClientInChatState : FsmState<Client>,
    IEventReceiver<NewChatMessageServerToClient>,
    IEventReceiver<TableState>
{
    [SerializeField] MainChatPanel chatPanel;
    
    public override void Enter()
    {
        base.Enter();
        
        Assert.IsNotNull(chatPanel);

        chatPanel.EnableGUI();
        chatPanel.ClearAllText();

        chatPanel.RegisterButtonSendClickAction(OnClickButtonSend);
        //chatPanel.RegisterButtonDisconnectClickAction(TODO);
    }

    public override void Exit()
    {
        base.Exit();
        
        chatPanel.DisableGUI();
        
        chatPanel.UnregisterButtonSendClickActions();
        chatPanel.UnregisterButtonDisconnectClickActions();
    }

    private void OnClickButtonSend()
    {
        string message = chatPanel.GetChatEntry();
        if (string.IsNullOrEmpty(message)) return;
        
        agent.connectionToServer.Send(new NewChatMessageClientToServer(message));
        chatPanel.SetChatEntry("");
    }
    
    public void On(TableState eventData)
    {
        foreach (string username in eventData.usernames)
        {
            chatPanel.AddUser(username);
        }

        foreach (string line in eventData.lines)
        {
            chatPanel.AddChatLine(line);
        }
    }

    public void On(NewChatMessageServerToClient eventData)
    {
        Assert.IsTrue(isEntered);
        
        chatPanel.AddChatLine(eventData.GetChatLine());
    }
}
