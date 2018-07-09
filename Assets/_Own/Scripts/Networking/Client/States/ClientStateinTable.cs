using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ClientStateinTable : FsmState<Client>,
    IEventReceiver<NewChatEntryMessage>,
    IEventReceiver<TableStateMessage>,
    IEventReceiver<UserJoinedMessage>,
    IEventReceiver<UserLeftMessage>
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
    
    public void On(TableStateMessage request)
    {
        Assert.IsTrue(isEntered);
        
        foreach (string username in request.nicknames)
        {
            chatPanel.AddUser(username);
        }

        foreach (string line in request.lines)
        {
            chatPanel.AddChatLine(line);
        }
    }

    public void On(NewChatEntryMessage request)
    {
        Assert.IsTrue(isEntered);

        var message = request.message;
        switch (request.kind)
        {
            case NewChatEntryMessage.Kind.ServerMessage:
                message = FormatServerMessage(message);
                break;
            default:
                break;
        }

        chatPanel.AddChatLine(message);
    }
    
    public void On(UserJoinedMessage request)
    {
        Assert.IsTrue(isEntered);

        chatPanel.AddUser(request.nickname);
    }

    public void On(UserLeftMessage request)
    {
        Assert.IsTrue(isEntered);
        
        chatPanel.RemoveUser(request.nickname);
    }

    private void OnClickButtonSend()
    {
        Debug.Log("OnClickButtonSend");
        
        string message = chatPanel.GetChatEntry();
        if (string.IsNullOrEmpty(message)) return;

        HandleMessageText(message);
        chatPanel.SetChatEntry("");
    }
    
    private void OnClickButtonDisconnect()
    {
        Debug.Log("OnClickButtonDisconnect");
        
        agent.connectionToServer.Send(new DisconnectMessage());
        agent.connectionToServer.Close();
    }

    private void HandleMessageText(string message)
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

    private static string FormatServerMessage(string message)
    {
        return $"<color=green>{message}</color>";
    }
}
