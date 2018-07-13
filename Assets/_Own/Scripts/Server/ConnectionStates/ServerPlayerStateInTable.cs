using System;
using UnityEngine;
using UnityEngine.Assertions;

/// When this state is active, Table is mostly running the show.
public class ServerPlayerStateInTable : FsmState<ServerPlayer>//,
    /*IEventReceiver<NewChatMessageClientToServer>,
    IEventReceiver<HelpRequest>*/
{
    public override void Enter()
    {
        base.Enter();
        
        //agent.messageFilter.Whitelist<NewChatMessageClientToServer>();
        //agent.messageFilter.Whitelist<HelpRequest>();
    }

    void Update()
    {
        /*if (Random.value < 0.1f)
        {
            var message = new NewChatMessageServerToClient("server", "howdy");
            agent.connection.Send(message);
        }*/
    }
    
    /*public void On(NewChatMessageClientToServer request)
    {
        if (request.originConnection != agent.connection) return;
        
        Assert.IsNotNull(agent.clientNickname);
        
        string chatMessage = $"{GetTimestampNow()} {agent.clientNickname}: {request.message}";
        var message = new NewChatEntryMessage(chatMessage);

        var server = Server.Instance;
        server.state.AddLine(chatMessage);
        server.SendAllClients(message);
    }

    public void On(HelpRequest request)
    {
        if (request.originConnection != agent.connection) return;

        const string HelpResponseMessage =
            "\n" +
            "Type a message to send it to everyone in the chatroom \n" +
            "\\help for help";

        string chatMessage = $"{GetTimestampNow()} {HelpResponseMessage}";

        var message = NewChatEntryMessage.MakeWithTimestamp(chatMessage, NewChatEntryMessage.Kind.ServerMessage);
        request.originConnection.Send(message);
    }

    private static string GetTimestampNow()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }*/
}