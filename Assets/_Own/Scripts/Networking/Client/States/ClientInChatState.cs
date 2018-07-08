using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ClientInChatState : FsmState<Client>,
    IEventReceiver<NewChatMessageServerToClient>,
    IEventReceiver<TableStateMessage>
{
    [SerializeField] MainChatPanel chatPanel;
    
    public override void Enter()
    {
        base.Enter();
        
        Assert.IsNotNull(chatPanel);

        chatPanel.EnableGUI();
        chatPanel.ClearAllText();
        chatPanel.RemoveAllUsers();

        chatPanel.RegisterButtonSendClickAction(OnClickButtonSend);
        chatPanel.RegisterButtonDisconnectClickAction(OnClickButtonDisconnect);
    }

    public override void Exit()
    {
        base.Exit();
        
        chatPanel.DisableGUI();
        
        chatPanel.UnregisterButtonSendClickActions();
        chatPanel.UnregisterButtonDisconnectClickActions();
    }
    
    public void On(TableStateMessage eventData)
    {        
        foreach (string username in eventData.nicknames)
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

        var message = eventData.message;
        switch (eventData.kind)
        {
            case NewChatMessageServerToClient.Kind.CommandResponse:
                message = $"<color=green>{message}</color>";
                break;
            default:
                break;
        }

        chatPanel.AddChatLine(message);
    }

    private void OnClickButtonSend()
    {
        Debug.Log("OnClickButtonSend");
        
        string message = chatPanel.GetChatEntry();
        if (string.IsNullOrEmpty(message)) return;

        SendMessage(message);
        chatPanel.SetChatEntry("");
    }
    
    private void OnClickButtonDisconnect()
    {
        Debug.Log("OnClickButtonDisconnect");
        
        agent.connectionToServer.Send(new DisconnectMessage());
        agent.connectionToServer.Close();
    }

    private void SendMessage(string message)
    {
        Connection connection = agent.connectionToServer;

        if (message.Trim() == "\\help")
        {
            connection.Send(new HelpRequest());
        }
        else
        {
            connection.Send(new NewChatMessageClientToServer(message));
        }
    }
}
