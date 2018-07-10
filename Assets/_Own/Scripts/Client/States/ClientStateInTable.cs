using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ClientStateInTable : FsmState<Client>,
    //IEventReceiver<NewChatEntryMessage>,
    IEventReceiver<NotifyTableState>,
    IEventReceiver<NotifyPlayerJoinedTable>,
    IEventReceiver<NotifyPlayerLeftTable>,
    IEventReceiver<NotifyGameStart>
{
    [SerializeField] MainChatPanel chatPanel;
    [SerializeField] BoardView boardView;
    
    public override void Enter()
    {
        base.Enter();
        
        Assert.IsNotNull(chatPanel);

        boardView.gameObject.SetActive(true);
        boardView.OnMoveRequest += OnMoveRequest;

        chatPanel.EnableGUI();
        chatPanel.ClearAllText();
        chatPanel.RemoveAllUsers();
        chatPanel.RegisterButtonSendClickAction(OnClickButtonSend);
        chatPanel.RegisterButtonDisconnectClickAction(OnClickButtonDisconnect);
    }

    public override void Exit()
    {
        base.Exit();

        boardView.Clear();
        boardView.gameObject.SetActive(false);
        
        chatPanel.DisableGUI();
        chatPanel.UnregisterButtonSendClickActions();
        chatPanel.UnregisterButtonDisconnectClickActions();
    }

    /*
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
    }*/
    
    public void On(NotifyTableState message)
    {
        boardView.SetCheckerboard(message.checkerboard);
    }
    
    public void On(NotifyPlayerJoinedTable message)
    {
        Assert.IsTrue(isEntered);

        chatPanel.AddUser(message.nickname);
    }

    public void On(NotifyPlayerLeftTable message)
    {
        Assert.IsTrue(isEntered);
        
        chatPanel.RemoveUser(message.nickname);
    }
    
    public void On(NotifyGameStart request)
    {
        throw new NotImplementedException();
        
        // TODO enable controls on the BoardView
    }

    private void OnMoveRequest(BoardView sender, Vector2Int origin, Vector2Int target)
    {
        agent.connectionToServer.Send(new MakeMove(origin, target));
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
