using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Table : MyBehaviour,
    IEventReceiver<MakeMove>
{
    private ServerPlayer playerA;
    private ServerPlayer playerB;
    private bool currentPlayerIsB;
    
    private readonly Checkerboard checkerboard = CheckersHelper.MakeDefaultCheckerboard();

    private bool isPlaying;

    public bool isFull => playerA && playerB;

    public void AddPlayer(ServerPlayer newPlayer)
    {
        Assert.IsNotNull(newPlayer);
        Assert.IsTrue(playerA == null || playerB == null);

        bool aIsNull = playerA == null;
        ServerPlayer otherPlayer = aIsNull ? playerB : playerA;
        
        if (aIsNull) playerA = newPlayer;
        else playerB = newPlayer;
        
        newPlayer.connection.Send(MakeTableStateMessage());
        newPlayer.connection.Send(MakeChatGreeting());
        if (otherPlayer) otherPlayer.connection.Send(new NotifyPlayerJoinedTable(newPlayer.playerId, newPlayer.nickname)); 
    }

    void FixedUpdate()
    {
        if (!isPlaying && isFull)
        {
            SendAllAtTable(new NotifyGameStart(playerA.playerId));
            isPlaying = true;
        }
    }
    
    private void SendAllAtTable(INetworkMessage message)
    {
        if (playerA) playerA.connection.Send(message);
        if (playerB) playerB.connection.Send(message);
    }
    
    private INetworkMessage MakeTableStateMessage()
    {
        return new NotifyTableState
        {
            checkerboard = checkerboard,
            playerANickname = playerA?.nickname,
            playerBNickname = playerB?.nickname
        };
    }
    
    private NewChatEntryMessage MakeChatGreeting()
    {
        const string WelcomeMessage =
            "\n" +
            "Welcome to the table! \n" +
            "Type `\\help` to get help.";

        return NewChatEntryMessage.MakeWithTimestamp(WelcomeMessage, NewChatEntryMessage.Kind.ServerMessage); 
    }

    public void On(MakeMove request)
    {
        if (!IsFromPlayersAtThisTable(request)) return;

        if (!IsFromCorrectPlayer(request))
        {
            Debug.LogWarning("Request to move a piece came from the wrong player.");
            return;
        }

        if (!checkerboard.TryMakeMove(request.origin, request.target))
        {
            Debug.LogWarning("Request to make invalid move.");
            return;
        }
        
        SendAllAtTable(new NotifyMakeMove(request.origin, request.target));
        currentPlayerIsB = !currentPlayerIsB;
        SendAllAtTable(new NotifyPlayerTurn(GetCurrentPlayer().playerId));
    }

    // TODO Have a system of multiple event queues, so you don't have to filter stuff out of the global one.
    private bool IsFromPlayersAtThisTable(INetworkMessage message)
    {
        if (!isFull) return false;
        Connection connection = message.originConnection;
        if (connection != playerA.connection && connection != playerB.connection) return false;

        return true;
    }

    private bool IsFromCorrectPlayer(MakeMove request)
    {
        return request.originConnection == GetCurrentPlayer().connection;
    }

    private ServerPlayer GetCurrentPlayer()
    {
        Assert.IsTrue(isFull);
        
        return currentPlayerIsB ? playerB : playerA;
    }
}