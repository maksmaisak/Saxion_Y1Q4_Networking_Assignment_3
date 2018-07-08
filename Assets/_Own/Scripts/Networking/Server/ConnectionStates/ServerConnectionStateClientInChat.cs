using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ServerConnectionStateClientInChat : FsmState<ServerSideConnectionHandler>,
    IEventReceiver<NewChatMessageClientToServer>,
    IEventReceiver<HelpRequest>
{    
    void Update()
    {
        /*if (Random.value < 0.1f)
        {
            var message = new NewChatMessageServerToClient("server", "howdy");
            agent.connection.Send(message);
        }*/
    }
    
    public void On(NewChatMessageClientToServer eventData)
    {
        if (eventData.originConnection != agent.connection) return;
        
        Assert.IsNotNull(agent.clientNickname);
        
        string chatMessage = $"{GetTimestampNow()} {agent.clientNickname}: {eventData.message}";
        var message = new NewChatMessageServerToClient(chatMessage);

        var server = Server.Instance;
        server.state.AddLine(chatMessage);
        server.SendAllClients(message);
    }

    public void On(HelpRequest eventData)
    {
        if (eventData.originConnection != agent.connection) return;

        const string HelpResponseMessage =
            "\n" +
            "Type a message to send it to everyone in the chatroom \n" +
            "\\help for help";

        string chatMessage = $"{GetTimestampNow()} {HelpResponseMessage}";

        var message =
            new NewChatMessageServerToClient(chatMessage, NewChatMessageServerToClient.Kind.CommandResponse);
        eventData.originConnection.Send(message);
    }

    private static string GetTimestampNow()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }
}