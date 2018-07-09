using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Table : MyBehaviour,
    IEventReceiver<MakeMove>
{
    private ServerPlayer playerA;
    private ServerPlayer playerB;
    
    private Checkerboard checkerboard = new Checkerboard();

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
        ServerPlayer other = aIsNull ? playerB : playerA;

        if (aIsNull) playerA = newPlayer;
        else playerB = newPlayer;
        newPlayer.connection.Send(MakeTableStateMessage());
        newPlayer.connection.Send(MakeChatGreeting());

        if (other) other.connection.Send(new NotifyPlayerJoinedTable(newPlayer.playerId, newPlayer.nickname));
    }

    public void SendAll(INetworkMessage message)
    {
        if (playerA) playerA.connection.Send(message);
        if (playerB) playerB.connection.Send(message);
    }
    
    private INetworkMessage MakeTableStateMessage()
    {
        throw new System.NotImplementedException();
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
        if (!isFull) return;
        var connection = request.originConnection;
        if (connection != playerA.connection && connection != playerB.connection) return;

        if (!checkerboard.IsValidMove(request.origin, request.destination));
    }
}