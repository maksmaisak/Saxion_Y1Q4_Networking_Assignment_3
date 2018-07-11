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
        
        if (playerA == null) playerA = newPlayer;
        else playerB = newPlayer;
        
        newPlayer.connection.Send(MakeTableStateMessage());
        
        SendAllAtTable(new NotifyPlayerJoinedTable(newPlayer.playerId, newPlayer.nickname)); 
        SendAllAtTable(MakeServerChatMessage($"{newPlayer.nickname}: joined the table"));
    }

    void FixedUpdate()
    {
        if (!isPlaying && isFull)
        {
            SendAllAtTable(new NotifyGameStart(playerA.playerId));
            SendAllAtTable(MakeServerChatMessage("The game is starting."));
            
            AnnounceNewTurn(GetCurrentPlayer());
            
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
        SendAllAtTable(MakeServerChatMessage($"{GetCurrentPlayer().nickname}: {request.origin} to {request.target}"));
        
        // TODO Check double capture
        // TODO Check victory
        
        currentPlayerIsB = !currentPlayerIsB;
        AnnounceNewTurn(GetCurrentPlayer());
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

    private void AnnounceNewTurn(ServerPlayer player)
    {
        SendAllAtTable(new NotifyPlayerTurn(player.playerId, player.nickname));
        SendAllAtTable(MakeServerChatMessage($"{player.nickname}'s turn."));
    }
    
    private NotifyChatEntryMessage MakeServerChatMessage(string message)
    {
        return NotifyChatEntryMessage.MakeWithTimestamp(message, NotifyChatEntryMessage.Kind.ServerMessage); 
    }
}