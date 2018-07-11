using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ClientStateInTable : FsmState<Client>,
    IEventReceiver<NotifyChatEntryMessage>,
    IEventReceiver<NotifyTableState>,
    IEventReceiver<NotifyPlayerJoinedTable>,
    IEventReceiver<NotifyPlayerLeftTable>,
    IEventReceiver<NotifyGameStart>,
    IEventReceiver<NotifyMakeMove>,
    IEventReceiver<NotifyPlayerTurn>
{
    [SerializeField] TableUIPanel uiPanel;
    [SerializeField] BoardView boardView;

    private Checkerboard checkerboard;
    
    public override void Enter()
    {
        base.Enter();
        
        Assert.IsNotNull(uiPanel);

        boardView.gameObject.SetActive(true);
        boardView.OnMoveRequest += OnMoveRequest;

        uiPanel.EnableGUI();
        uiPanel.ClearAllText();
        uiPanel.RegisterButtonSendClickAction(OnClickButtonSend);
        uiPanel.RegisterButtonDisconnectClickAction(OnClickButtonDisconnect);
    }

    public override void Exit()
    {
        base.Exit();

        boardView.Clear();
        boardView.gameObject.SetActive(false);
        
        uiPanel.DisableGUI();
        uiPanel.UnregisterButtonSendClickActions();
        uiPanel.UnregisterButtonDisconnectClickActions();
    }

    public void On(NotifyChatEntryMessage request)
    {
        Assert.IsTrue(isEntered);

        var message = request.message;
        switch (request.kind)
        {
            case NotifyChatEntryMessage.Kind.ServerMessage:
                message = FormatServerMessage(message);
                break;
            default:
                break;
        }

        uiPanel.AddChatLine(message);
    }
        
    public void On(NotifyTableState message)
    {
        checkerboard = message.checkerboard;
        boardView.SetCheckerboard(checkerboard);
    }
    
    public void On(NotifyPlayerJoinedTable message)
    {
        Assert.IsTrue(isEntered);
    }

    public void On(NotifyPlayerLeftTable message)
    {
        Assert.IsTrue(isEntered);
    }
    
    public void On(NotifyGameStart message)
    {
        Assert.IsTrue(isEntered);
    }
    
    public void On(NotifyPlayerTurn message)
    {
        bool isOwnTurn = agent.playerId == message.playerId;
        boardView.SetControlsEnabled(isOwnTurn);
        uiPanel.SetStatusText(isOwnTurn ? "Your turn" : $"{message.playerNickname}'s turn");
    }
    
    public void On(NotifyMakeMove message)
    {
        bool didSucceed = checkerboard.TryMakeMove(message.origin, message.target);
        Assert.IsTrue(didSucceed);
    }

    private void OnMoveRequest(BoardView sender, Vector2Int origin, Vector2Int target)
    {
        sender.SetControlsEnabled(false);
        agent.connectionToServer.Send(new MakeMove(origin, target));
    }

    private void OnClickButtonSend()
    {
        Debug.Log("OnClickButtonSend");
        
        string message = uiPanel.GetChatEntry();
        if (string.IsNullOrEmpty(message)) return;

        HandleMessageText(message);
        uiPanel.SetChatEntry("");
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

    private void WriteToChatAsSystem(string message)
    {
        uiPanel.AddChatLine(FormatServerMessage(message));
    }

    private static string FormatServerMessage(string message)
    {
        return $"<color=green>{message}</color>";
    }
}
