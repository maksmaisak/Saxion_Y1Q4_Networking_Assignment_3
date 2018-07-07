using System;
using UnityEngine;
using UnityEngine.Assertions;

public class AfterClientJoinedChat : FsmState<ServerSideConnectionHandler>,
    IEventReceiver<NewChatMessageClientToServer>
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
        var message = new NewChatMessageServerToClient
        {
            timestamp = DateTime.Now,
            nickname = agent.clientNickname,
            message = eventData.message
        };
        
        Server.Instance.SendAllClients(message);
    }
}