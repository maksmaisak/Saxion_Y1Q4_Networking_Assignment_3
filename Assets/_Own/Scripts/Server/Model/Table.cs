﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Table : MyBehaviour,
    IEventReceiver<MakeMove>
{
    private ServerPlayer playerA;
    private ServerPlayer playerB;
    private bool currentPlayerIsB;
    
    private Checkerboard checkerboard = CheckersHelper.MakeDefaultCheckerboard();

    private bool isPlaying;

    public bool isFull => playerA && playerB;

    public void AddPlayer(ServerPlayer newPlayer)
    {
        Assert.IsNotNull(newPlayer);
        Assert.IsTrue(playerA == null || playerB == null);

        if (playerA == null) playerA = newPlayer;
        else playerB = newPlayer;
        
        SendTableStateMessage(newPlayer);
        
        SendAllAtTable(new NotifyPlayerJoinedTable(GetPlayerInfo(newPlayer))); 
        SendAllAtTable(MakeServerChatMessage($"{newPlayer.nickname}: joined the table"));
    }

    void FixedUpdate()
    {
        if (!isPlaying && isFull)
        {
            StartNewGame();
            AnnounceNewTurn(GetCurrentPlayer());
            
            isPlaying = true;
        }
    }
    
    private void SendAllAtTable(INetworkMessage message)
    {
        if (playerA) playerA.connection.Send(message);
        if (playerB) playerB.connection.Send(message);
    }
    
    private void SendTableStateMessage(ServerPlayer newPlayer)
    {
        var message = new NotifyTableState(checkerboard);
        
        ServerPlayer otherPlayer = newPlayer == playerA ? playerB : playerA;
        if (otherPlayer)
        {
            message.otherPlayerInfo = GetPlayerInfo(otherPlayer);
        }

        newPlayer.connection.Send(message);
    }

    private static PlayerInfo GetPlayerInfo(ServerPlayer player)
    {
        return new PlayerInfo
        {
            id = player.playerId,
            nickname = player.nickname
        };
    }

    public void On(MakeMove request)
    {
        if (!IsFromPlayersAtThisTable(request)) return;

        if (!IsFromCorrectPlayer(request))
        {
            Debug.LogWarning("Request to move a piece came from the wrong player.");
            SendAllAtTable(MakeServerChatMessage("Warning: Request to make invalid move."));
            return;
        }

        if (!checkerboard.TryMakeMove(request.origin, request.target))
        {
            Debug.LogWarning("Request to make invalid move.");
            SendAllAtTable(MakeServerChatMessage("Warning: Request to make invalid move."));
            return;
        }

        AnnounceMove(request.origin, request.target);
            
        Checkerboard.TileState victorColor = checkerboard.CheckVictory();
        if (victorColor != Checkerboard.TileState.None)
        {
            ServerPlayer victor = victorColor == Checkerboard.TileState.White ? playerA : playerB;
            SendAllAtTable(new NotifyVictory(victor.playerId));
            SendAllAtTable(MakeServerChatMessage($"{victor.nickname} wins!"));

            checkerboard = CheckersHelper.MakeDefaultCheckerboard();
            StartNewGame();
            AnnounceNewTurn(GetCurrentPlayer());
                            
            return;
        }
        
        // TODO Check double capture
        // only change if not double capture
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
    
    private void StartNewGame()
    {
        checkerboard = CheckersHelper.MakeDefaultCheckerboard();
        SendAllAtTable(new NotifyGameStart(checkerboard, playerA.playerId));
        SendAllAtTable(MakeServerChatMessage("The game is starting."));
    }

    private void AnnounceMove(Vector2Int origin, Vector2Int target)
    {
        SendAllAtTable(new NotifyMakeMove(origin, target));
        SendAllAtTable(MakeServerChatMessage($"{GetCurrentPlayer().nickname}: {origin} to {target}"));
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