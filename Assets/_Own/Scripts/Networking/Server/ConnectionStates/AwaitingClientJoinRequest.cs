using System.Linq;
using UnityEngine;

public class AwaitingClientJoinRequest : FsmState<ServerSideConnectionHandler>,
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
            agent.connection.Close();
            return;
        }
        
        // TODO check if request.nickName is valid. If not, send a reject and close the connection. 
        // TODO ? Have a state to do the disconnecting.

        server.state.AddNickname(request.nickname);
        agent.clientNickname = request.nickname;

        agent.connection.Send(JoinChatResponse.Accept);
        agent.connection.Send(new TableStateMessage
        {
            nicknames = server.state.GetNicknames().ToArray(), 
            lines = server.state.GetLines().ToArray()
        });
        
        agent.fsm.ChangeState<AfterClientJoinedChat>();
    }
}