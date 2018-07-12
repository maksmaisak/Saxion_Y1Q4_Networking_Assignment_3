using System.Linq;
using UnityEngine;

public class ServerPlayerStateInLobby : FsmState<ServerPlayer>,
    IEventReceiver<JoinTableRequest>
{   
    public void On(JoinTableRequest request)
    {
        if (request.originConnection != agent.connection) return;
        if (RejectIfNeeded(request)) return;
        
        agent.connection.Send(JoinTableResponse.Accept);
        
        agent.table = Server.Instance.GetNonFullTable();
        agent.table.AddPlayer(agent);
        
        agent.fsm.ChangeState<ServerPlayerStateInTable>();
    }

    private bool RejectIfNeeded(JoinTableRequest request)
    {
        var table = Server.Instance.GetNonFullTable();
        if (!table || table.isFull)
        {
            var message = JoinTableResponse.MakeReject("The table is already full.");
            agent.connection.Send(message);
            return true;
        }

        return false;
    }
}