using System.Collections;
using UnityEngine;

public class AwaitingClientHandshake : FsmState<ServerSideConnectionHandler>, 
    IEventReceiver<ClientHandshake>
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

    public void On(ClientHandshake handshake)
    {
        if (handshake.originConnection != agent.connection) return;
        
        try
        {
            handshake.ThrowIfInvalidProtocolIdentifier();
            Debug.Log("Valid client handshake received.");
            agent.fsm.ChangeState<AwaitingClientJoinRequest>();
        }
        catch (ClientHandshake.InvalidClientHandshakeException)
        {
            Debug.Log("Invalid client handshake. Kicking client.");
            KickClient();
        }
    }

    private void KickClient()
    {
        agent.connection.Send(new DisconnectMessage());
        agent.connection.Close();
    }
}