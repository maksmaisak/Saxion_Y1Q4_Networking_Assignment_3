using System.Linq;
using UnityEngine;

public class ServerConnectionStateAwaitingClientJoinRequest : FsmState<ServerSideConnectionHandler>,
    IEventReceiver<JoinChatRequest>
{
    public void On(JoinChatRequest request)
    {
        if (request.originConnection != agent.connection) return;

        Server server = Server.Instance;
        if (server.state.HasNickname(request.nickname))
        {
            var message = JoinChatResponse.MakeReject($"Username {request.nickname} is already taken.");
            agent.connection.Send(message);
            return;
        }
        
        agent.connection.Send(JoinChatResponse.Accept);
        agent.connection.Send(MakeCurrentTableStateMessage());
        server.state.AddNickname(request.nickname);
        agent.connection.Send(MakeGreeting());
        
        agent.clientNickname = request.nickname;
        agent.fsm.ChangeState<ServerConnectionStateClientInChat>();
    }

    private TableStateMessage MakeCurrentTableStateMessage()
    {
        ChatboxState state = Server.Instance.state;
        return new TableStateMessage
        {
            nicknames = state.GetNicknames().ToArray(),
            lines = state.GetLines().ToArray()
        };
    }

    private NewChatEntryMessage MakeGreeting()
    {
        const string WelcomeMessage =
            "\n" +
            "Welcome to the server! \n" +
            "Type `\\help` to get help.";

        return NewChatEntryMessage.MakeWithTimestamp(WelcomeMessage, NewChatEntryMessage.Kind.ServerMessage); 
    }
}