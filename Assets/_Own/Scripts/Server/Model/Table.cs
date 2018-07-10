using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Table : MyBehaviour,
    IEventReceiver<MakeMove>
{
    private ServerPlayer playerA;
    private ServerPlayer playerB;
    
    private readonly Checkerboard checkerboard = new Checkerboard();

    private bool isPlaying;

    public bool isFull
    {
        get
        {
            return playerA != null && playerB != null;
        }
    }

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

    public void SendAllAtTable(INetworkMessage message)
    {
        if (playerA) playerA.connection.Send(message);
        if (playerB) playerB.connection.Send(message);
    }

    void FixedUpdate()
    {
        if (!isPlaying && isFull)
        {
            SendAllAtTable(new NotifyGameStart());
            isPlaying = true;
        }
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
            "Welcome to the server! \n" +
            "Type `\\help` to get help.";

        return NewChatEntryMessage.MakeWithTimestamp(WelcomeMessage, NewChatEntryMessage.Kind.ServerMessage); 
    }

    public void On(MakeMove request)
    {
        if (!IsFromPlayersAtThisTable(request)) return;

        if (!checkerboard.TryMakeMove(request.origin, request.target))
        {
            // TODO handle invalid move
            return;
        }
        
        SendAllAtTable(new NotifyMakeMove(request.origin, request.target));
    }

    // TODO Have a system of multiple event queues, so you don't have to filter stuff out of the global one.
    private bool IsFromPlayersAtThisTable(INetworkMessage message)
    {
        if (!isFull) return false;
        Connection connection = message.originConnection;
        if (connection != playerA.connection && connection != playerB.connection) return false;

        return true;
    }
}