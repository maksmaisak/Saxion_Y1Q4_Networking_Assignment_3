using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

public class ServerPlayerStateAwaitingServerJoinRequest : FsmState<ServerPlayer>, 
    IEventReceiver<JoinServerRequest>
{
    [SerializeField] float handshakeTimeout = 1f;
    
    private float timeLeftForHandshake;
    
    public override void Enter()
    {
        base.Enter();
        timeLeftForHandshake = handshakeTimeout;
    }

    void FixedUpdate()
    {
        timeLeftForHandshake -= Time.fixedDeltaTime;
        if (timeLeftForHandshake > 0f) return;

        Debug.Log("Client handshake timeout. Kicking client.");
        KickClient();
    }

    public void On(JoinServerRequest request)
    {
        if (request.originConnection != agent.connection) return;
        if (RejectIfNeeded(request)) return;
        
        AcceptClient(request);
        agent.fsm.ChangeState<ServerPlayerStateInLobby>();
    }

    private bool RejectIfNeeded(JoinServerRequest request)
    {
        if (!request.GetIsValid())
        {
            agent.connection.Send(JoinServerResponse.MakeReject("Invalid JoinServerRequest."));
            KickClient();
            return true;
        }
        
        Debug.Log("Valid client handshake received.");
        
        if (Server.Instance.joinedPlayers.Any(p => p.nickname == request.nickname))
        {
            agent.connection.Send(JoinTableResponse.MakeReject($"Username {request.nickname} is already taken."));
            KickClient();
            return true;
        }

        return false;
    }
    
    private void AcceptClient(JoinServerRequest request)
    {
        Server.Instance.joinedPlayers.Add(agent);
        agent.nickname = request.nickname;
        agent.playerId = Server.Instance.GetNextPlayerId();

        agent.connection.Send(JoinServerResponse.MakeAccept(agent.playerId));
    }
    
    private void KickClient()
    {
        agent.connection.Send(new DisconnectMessage());
        agent.connection.Close();
    }
}