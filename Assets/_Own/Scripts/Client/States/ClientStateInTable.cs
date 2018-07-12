using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ClientStateInTable : FsmState<Client>,
    IEventReceiver<NotifyChatEntryMessage>,
    IEventReceiver<NotifyTableState>,
    IEventReceiver<NotifyPlayerJoinedTable>,
    IEventReceiver<NotifyPlayerLeftTable>,
    IEventReceiver<NotifyGameStart>,
    IEventReceiver<NotifyMakeMove>,
    IEventReceiver<NotifyPlayerTurn>,
    IEventReceiver<NotifyVictory>
{
    [SerializeField] TableUIPanel uiPanel;
    [SerializeField] CheckerboardView checkerboardView;

    private Checkerboard checkerboard;
    private PlayerInfo? otherPlayerInfo;
    
    public override void Enter()
    {
        base.Enter();
        Assert.IsNotNull(uiPanel);

        checkerboardView.gameObject.SetActive(true);
        checkerboardView.SetControlsEnabled(false);
        checkerboardView.OnMoveRequest += OnMoveRequest;

        uiPanel.EnableGUI();
        uiPanel.ClearAllText();
        uiPanel.RegisterButtonSendClickAction(OnClickButtonSend);
        uiPanel.RegisterButtonDisconnectClickAction(OnClickButtonDisconnect);
        
        uiPanel.SetStatusText("Awaiting the other player");
    }

    public override void Exit()
    {
        base.Exit();

        checkerboardView.Clear();
        checkerboardView.gameObject.SetActive(false);
        
        uiPanel.DisableGUI();
        uiPanel.UnregisterButtonSendClickActions();
        uiPanel.UnregisterButtonDisconnectClickActions();
    }

    public void On(NotifyChatEntryMessage message)
    {
        Assert.IsTrue(isEntered);

        var text = message.message;
        switch (message.kind)
        {
            case NotifyChatEntryMessage.Kind.ServerMessage:
                text = FormatServerMessage(text);
                break;
            default:
                break;
        }

        uiPanel.AddChatLine(text);
    }
        
    public void On(NotifyTableState message)
    {
        Assert.IsTrue(isEntered);

        checkerboard = message.checkerboard;
        checkerboardView.SetCheckerboard(checkerboard);

        otherPlayerInfo = message.otherPlayerInfo;
    }
    
    public void On(NotifyPlayerJoinedTable message)
    {
        Assert.IsTrue(isEntered);
        if (message.playerInfo.id == agent.playerInfo.id) return;

        otherPlayerInfo = message.playerInfo;
    }

    public void On(NotifyPlayerLeftTable message)
    {
        Assert.IsTrue(isEntered);

        if (otherPlayerInfo?.id == message.playerId)
        {
            otherPlayerInfo = null;
        }
    }
    
    public void On(NotifyGameStart message)
    {
        Assert.IsTrue(isEntered);

        checkerboard = message.checkerboard;
        checkerboardView.Clear();
        checkerboardView.SetCheckerboard(checkerboard);

        bool isThisWhite = message.whitePlayerId == agent.playerInfo.id;
        checkerboardView.SetOwnColor(isThisWhite ? Checkerboard.TileState.White : Checkerboard.TileState.Black);
        checkerboardView.SetControlsEnabled(true);
    }
    
    public void On(NotifyPlayerTurn message)
    {
        Assert.IsTrue(isEntered);
        Assert.IsTrue(message.playerId == agent.playerInfo.id || message.playerId == otherPlayerInfo?.id);
        
        bool isOwnTurn = agent.playerInfo.id == message.playerId;
        checkerboardView.SetControlsEnabled(isOwnTurn);
        uiPanel.SetStatusText(isOwnTurn ? "Your turn" : $"{otherPlayerInfo?.nickname}'s turn");
    }
    
    public void On(NotifyMakeMove message)
    {
        Assert.IsTrue(isEntered);
        
        bool didSucceed = checkerboard.TryMakeMove(message.origin, message.target);
        Assert.IsTrue(didSucceed);
    }
    
    public void On(NotifyVictory message)
    {
        Assert.IsTrue(isEntered);
        Assert.IsTrue(otherPlayerInfo.HasValue);

        bool didWin = message.playerId == agent.playerInfo.id;
        uiPanel.SetStatusText(didWin ? agent.playerInfo.nickname : otherPlayerInfo.Value.nickname + "Won!");
    }

    private void OnMoveRequest(CheckerboardView sender, Vector2Int origin, Vector2Int target)
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
